import {Component,computed,inject,OnInit,signal} from '@angular/core';
import {HttpErrorResponse} from '@angular/common/http';
import {FormBuilder,ReactiveFormsModule,Validators} from '@angular/forms';
import {finalize} from 'rxjs';
import {ConfirmationService,MessageService} from 'primeng/api';
import {ButtonModule} from 'primeng/button';
import {CheckboxModule} from 'primeng/checkbox';
import {ConfirmDialogModule} from 'primeng/confirmdialog';
import {DialogModule} from 'primeng/dialog';
import {InputTextModule} from 'primeng/inputtext';
import {TableModule} from 'primeng/table';
import {TagModule} from 'primeng/tag';
import {ToastModule} from 'primeng/toast';
import {AuditHistoryComponent} from '../../../shared/audit-history/audit-history.component';
import {PermissionCodes} from '../../../core/permissions/constants/permission-codes';
import {PermissionService} from '../../../core/permissions/services/permission.service';
import {ProcedureTypeItem,ProcedureTypeQuery} from '../models/procedure-types.model';
import {ProcedureTypeService} from '../services/procedure-types.service';
/** Tetkik Türü Tanımları: eski enum kodu değişmez, ad ve durum veritabanından yönetilir. */
@Component({
 selector:'app-procedure-type-list',standalone:true,
 imports:[ReactiveFormsModule,ButtonModule,CheckboxModule,ConfirmDialogModule,
  DialogModule,InputTextModule,TableModule,TagModule,ToastModule,AuditHistoryComponent],
 providers:[ConfirmationService,MessageService],
 templateUrl:'./type-list.component.html',
 styleUrl:'./type-list.component.scss'
})
export class ProcedureTypeListComponent implements OnInit {
 private readonly api=inject(ProcedureTypeService);
 private readonly permissions=inject(PermissionService);
 private readonly fb=inject(FormBuilder);
 private readonly confirm=inject(ConfirmationService);
 private readonly messages=inject(MessageService);
 readonly canCreate=computed(()=>this.permissions.has(PermissionCodes.ProcedureTypes.Create));
 readonly canUpdate=computed(()=>this.permissions.has(PermissionCodes.ProcedureTypes.Update));
 readonly canDelete=computed(()=>this.permissions.has(PermissionCodes.ProcedureTypes.Delete));
 readonly canExport=computed(()=>this.permissions.has(PermissionCodes.ProcedureTypes.Query));
 readonly canAudit=computed(()=>this.permissions.has(PermissionCodes.AuditLogs.View));
 readonly rows=signal<ProcedureTypeItem[]>([]);
 readonly total=signal(0);readonly page=signal(1);readonly size=signal(20);
 readonly maxPage=computed(()=>Math.max(1,Math.ceil(this.total()/this.size())));
 readonly loading=signal(false);readonly saving=signal(false);readonly exporting=signal(false);
 readonly editing=signal<ProcedureTypeItem|null>(null);
 readonly audit=signal<ProcedureTypeItem|null>(null);
 readonly dialogVisible=signal(false);
 readonly attempted=signal(false);
 readonly error=signal<string|null>(null);
 readonly filtersVisible=signal(false);
 readonly filterForm=this.fb.nonNullable.group({search:'',includeInactive:false});
 readonly columnForm=this.fb.group({isActive:this.fb.control<boolean|null>(null)});
 readonly form=this.fb.group({
  code:this.fb.control<number|null>(null,{validators:[Validators.required,Validators.min(0)]}),
  name:this.fb.nonNullable.control('',[Validators.required,Validators.maxLength(200)])
 });
 private requestId=0;
 ngOnInit():void {this.load();}
 private query():ProcedureTypeQuery{
  const q=this.filterForm.getRawValue();
  const f=this.columnForm.getRawValue();
  return {search:q.search,includeInactive:q.includeInactive||f.isActive===false,isActive:f.isActive,
   pageNumber:this.page(),pageSize:this.size()};
 }
 /** Backend'de filtre ve sayfalama, PrimeNG'de kolon sıralaması yapılır. */
 load():void{
  const rid=++this.requestId;this.loading.set(true);
  this.api.list(this.query()).pipe(finalize(()=>{if(rid===this.requestId)this.loading.set(false);}))
  .subscribe({next:r=>{if(rid===this.requestId){this.rows.set(r.items);this.total.set(r.totalCount);}},
   error:e=>{if(rid===this.requestId){this.rows.set([]);this.total.set(0);this.notice(e);}}});
 }
 search():void{this.page.set(1);this.load();}
 clear():void{this.filterForm.reset({search:'',includeInactive:false});this.columnForm.reset({isActive:null});this.search();}
 setPage(n:number):void{if(n<1||n>this.maxPage()||this.loading())return;this.page.set(n);this.load();}
 setSize(e:Event):void{this.size.set(Number((e.target as HTMLSelectElement).value));this.search();}
 create():void{
  this.form.controls.code.enable();
  this.editing.set(null);this.form.reset({code:null,name:''});this.attempted.set(false);
  this.error.set(null);this.dialogVisible.set(true);
 }
 edit(item:ProcedureTypeItem):void{
  this.api.getById(item.id).subscribe({next:d=>{
    this.form.controls.code.disable();
    this.editing.set(d);this.form.reset({code:d.code,name:d.name});
    this.attempted.set(false);this.error.set(null);this.dialogVisible.set(true);
   },error:e=>this.notice(e)});
 }
 save():void{
  if(this.saving()||(this.editing()?!this.canUpdate():!this.canCreate()))return;
  this.attempted.set(true);this.error.set(null);
  if(this.form.invalid){this.form.markAllAsTouched();return;}
  const d=this.form.getRawValue(),old=this.editing();
  const request=old?this.api.update(old.id,old.version,d.name!.trim()):
    this.api.create(d.code!,d.name!.trim());
  this.saving.set(true);
  request.pipe(finalize(()=>this.saving.set(false))).subscribe({
    next:()=>{this.dialogVisible.set(false);this.messages.add({severity:'success',summary:'Kaydedildi'});this.load();},
    error:e=>{this.error.set(this.errorText(e));this.notice(e);}
  });
 }
 status(item:ProcedureTypeItem):void{
  if(!this.canUpdate())return;
  this.confirm.confirm({header:'Durum Değiştir',message:'Tetkik türünün durumu değiştirilsin mi?',
    acceptLabel:'Onayla',rejectLabel:'Vazgeç',accept:()=>this.api.status(item.id,item.version,!item.isActive)
       .subscribe({next:()=>this.load(),error:e=>this.notice(e)})});
 }
 delete(item:ProcedureTypeItem):void{
  if(!this.canDelete())return;
  this.confirm.confirm({header:'Silme Onayı',message:'Tetkik türü silinsin mi? Kullanımdaki türler silinemez.',
   acceptLabel:'Sil',rejectLabel:'Vazgeç',accept:()=>this.api.delete(item.id,item.version)
    .subscribe({next:()=>this.load(),error:e=>this.notice(e)})});
 }
 exportExcel():void{
  if(!this.canExport())return;
  this.exporting.set(true);
  this.api.export(this.query()).pipe(finalize(()=>this.exporting.set(false))).subscribe({
   next:blob=>{const u=URL.createObjectURL(blob),a=document.createElement('a');a.href=u;
       a.download='Tetkik_Turu_Tanimlari.xlsx';a.click();setTimeout(()=>URL.revokeObjectURL(u),1000);},
   error:e=>this.notice(e)});
 }
 private errorText(e:unknown):string{
  const r=e as HttpErrorResponse;return r.error?.detail||r.error?.title||r.message||'İşlem gerçekleştirilemedi.';
 }
 private notice(e:unknown):void{this.messages.add({severity:'error',summary:'İşlem başarısız',detail:this.errorText(e),life:7000});}
}

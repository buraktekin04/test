import {Component,computed,inject,OnInit,signal} from '@angular/core';
import {DatePipe} from '@angular/common';
import {HttpErrorResponse} from '@angular/common/http';
import {FormBuilder,ReactiveFormsModule,Validators} from '@angular/forms';
import {Router} from '@angular/router';
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
import {ValidityPeriodUtils} from '../../../core/utils/validity-period.utils';
import {ReasonItem,ReasonFields,ReasonQuery,RelationKind} from '../models/reason.model';
import {ReasonsService} from '../services/reasons.service';
type Field='code'|'description'|'validFrom'|'validTo';
const relations:{kind:RelationKind;label:string;slug:string;module:keyof typeof PermissionCodes}[]=[
 {kind:'reportTypes',label:'Rapor Türleri',slug:'reason-exam-report-types',module:'ReasonForExamReportTypes'},
 {kind:'unusedAdmissionTypes',label:'Kullanılmayacak Hasta Grupları',slug:'reason-exam-unused-admission-types',module:'ReasonForExamUnusedHcAdmissionTypes'},
 {kind:'procedures',label:'Gereken Tetkikler',slug:'reason-exam-procedures',module:'ReasonForExamProcedureDefs'},
 {kind:'branches',label:'Muayene Branşları',slug:'reason-exam-branches',module:'ReasonForExamBranchDefs'}
];
/** Muayene Sebebi ana tanım ekranı; aktif ilişki adlarını tek satırda gösterir. */
@Component({
 selector:'app-reason-list',standalone:true,
 imports:[DatePipe,ReactiveFormsModule,ButtonModule,CheckboxModule,ConfirmDialogModule,
 DialogModule,InputTextModule,TableModule,TagModule,ToastModule,AuditHistoryComponent],
 providers:[ConfirmationService,MessageService],
 templateUrl:'./reason-list.component.html',styleUrl:'./reason-list.component.scss'
})
export class ReasonListComponent implements OnInit {
 private readonly service=inject(ReasonsService);
 private readonly permissions=inject(PermissionService);
 private readonly fb=inject(FormBuilder);
 private readonly confirm=inject(ConfirmationService);
 private readonly toast=inject(MessageService);
 private readonly router=inject(Router);
 readonly relationColumns=relations;
 readonly canCreate=computed(()=>this.permissions.has(PermissionCodes.ReasonsForExaminations.Create));
 readonly canUpdate=computed(()=>this.permissions.has(PermissionCodes.ReasonsForExaminations.Update));
 readonly canDelete=computed(()=>this.permissions.has(PermissionCodes.ReasonsForExaminations.Delete));
 readonly canExport=computed(()=>this.permissions.has(PermissionCodes.ReasonsForExaminations.Query));
 readonly canAudit=computed(()=>this.permissions.has(PermissionCodes.AuditLogs.View));
 readonly records=signal<ReasonItem[]>([]);
 readonly totalCount=signal(0);readonly pageNumber=signal(1);readonly pageSize=signal(20);
 readonly loading=signal(false);readonly saving=signal(false);readonly exporting=signal(false);
 readonly dialogVisible=signal(false);readonly editing=signal<ReasonItem|null>(null);
 readonly auditRecord=signal<ReasonItem|null>(null);readonly filtersVisible=signal(false);
 readonly attemptedSave=signal(false);readonly formError=signal<string|null>(null);
 readonly totalPages=computed(()=>Math.max(1,Math.ceil(this.totalCount()/this.pageSize())));
 readonly rangeStart=computed(()=>this.totalCount()?(this.pageNumber()-1)*this.pageSize()+1:0);
 readonly rangeEnd=computed(()=>Math.min(this.totalCount(),this.pageNumber()*this.pageSize()));
 readonly filterForm=this.fb.nonNullable.group({search:'',includeInactive:false});
 readonly columnFilterForm=this.fb.group({isActive:this.fb.control<boolean|null>(null)});
 readonly itemForm=this.fb.nonNullable.group({
  code:['',[Validators.required,Validators.maxLength(100)]],
  description:['',[Validators.required,Validators.maxLength(500)]],
  dispatchRequired:false,classificationExamination:false,objectionable:false,
  validFrom:'',validTo:''
 });
 private requestId=0;
 ngOnInit():void{this.load();}
 canOpenRelation(module:keyof typeof PermissionCodes):boolean{return this.permissions.hasModule(module);}
 private query():ReasonQuery {const x=this.filterForm.getRawValue(),f=this.columnFilterForm.getRawValue();
 return {search:x.search,includeInactive:x.includeInactive||f.isActive===false,isActive:f.isActive,
 pageNumber:this.pageNumber(),pageSize:this.pageSize()};}
 /** Filtre ve sayfalama sunucu tarafında, kolon sıralaması p-table üzerinde yapılır. */
 load():void{
 const id=++this.requestId;this.loading.set(true);
 this.service.list(this.query()).pipe(finalize(()=>{if(id===this.requestId)this.loading.set(false);}))
 .subscribe({next:r=>{if(id!==this.requestId)return;this.records.set(r.items);this.totalCount.set(r.totalCount);},
 error:e=>{if(id!==this.requestId)return;this.notice(e);this.records.set([]);this.totalCount.set(0);}});
 }
 search():void{this.pageNumber.set(1);this.load();}
 clear():void{this.filterForm.reset({search:'',includeInactive:false});this.columnFilterForm.reset({isActive:null});this.search();}
 page(p:number):void{if(p<1||p>this.totalPages()||this.loading())return;this.pageNumber.set(p);this.load();}
 size(event:Event):void{const n=Number((event.target as HTMLSelectElement).value);
 if(![10,20,50,100].includes(n))return;this.pageSize.set(n);this.search();}
 openCreate():void{this.editing.set(null);this.attemptedSave.set(false);this.formError.set(null);
 this.itemForm.reset({code:'',description:'',dispatchRequired:false,classificationExamination:false,objectionable:false,validFrom:'',validTo:''});
 this.dialogVisible.set(true);}
 openEdit(item:ReasonItem):void{
 this.service.getById(item.id).subscribe({next:d=>{this.editing.set(d);this.formError.set(null);this.attemptedSave.set(false);
 this.itemForm.reset({code:d.code,description:d.description,dispatchRequired:d.dispatchRequired,
 classificationExamination:d.classificationExamination,objectionable:d.objectionable,
 validFrom:ValidityPeriodUtils.toLocalInput(d.validFrom),validTo:ValidityPeriodUtils.toLocalInput(d.validTo)});
 this.dialogVisible.set(true);},error:e=>this.notice(e)});
 }
 fieldError(field:Field):string|null{const control=this.itemForm.controls[field];
 if(!this.attemptedSave()&&!control.touched)return null;
 if(control.hasError('required'))return 'Bu alan zorunludur.';
 if(control.hasError('maxlength'))return 'Karakter sınırı aşıldı.';
 if(field==='validTo'&&!ValidityPeriodUtils.hasValidRange(this.itemForm.value.validFrom??'',this.itemForm.value.validTo??''))
 return 'Bitiş tarihi başlangıçtan sonra olmalıdır.';
 return null;}
 save():void{
 if(this.saving()||(this.editing()?!this.canUpdate():!this.canCreate()))return;
 this.attemptedSave.set(true);this.formError.set(null);
 const v=this.itemForm.getRawValue();
 if(this.itemForm.invalid||!ValidityPeriodUtils.hasValidRange(v.validFrom,v.validTo)){
 this.itemForm.markAllAsTouched();this.formError.set('Lütfen zorunlu alanları ve tarih aralığını kontrol ediniz.');
 return;}
 const fields:ReasonFields={code:v.code.trim().toUpperCase(),description:v.description.trim(),
 dispatchRequired:v.dispatchRequired,classificationExamination:v.classificationExamination,
 objectionable:v.objectionable,...ValidityPeriodUtils.fromLocalInputs(v.validFrom,v.validTo)};
 const edit=this.editing();
 const req=edit?this.service.update({...fields,id:edit.id,version:edit.version}):this.service.create(fields);
 this.saving.set(true);req.pipe(finalize(()=>this.saving.set(false))).subscribe({
 next:()=>{this.dialogVisible.set(false);this.toast.add({severity:'success',summary:'Kaydedildi',detail:'Muayene sebebi kaydedildi.'});this.load();},
 error:e=>{this.formError.set(this.errorText(e));this.notice(e);}
 });
 }
 changeStatus(item:ReasonItem):void{
 if(!this.canUpdate())return;
 this.confirm.confirm({header:'Durum Değiştir',message:'Seçili muayene sebebinin durumunu değiştirmek istiyor musunuz?',
 acceptLabel:'Onayla',rejectLabel:'Vazgeç',accept:()=>{
 this.service.changeStatus(item.id,item.version,!item.isActive).subscribe({next:()=>this.load(),error:e=>this.notice(e)});}});
 }
 delete(item:ReasonItem):void{
 if(!this.canDelete())return;
 this.confirm.confirm({header:'Silme Onayı',message:'Muayene sebebi silinsin mi? Bağlı tanımlar varsa işlem engellenir.',
 acceptLabel:'Sil',rejectLabel:'Vazgeç',accept:()=>{
 this.service.delete(item.id,item.version).subscribe({next:()=>this.load(),error:e=>this.notice(e)});}});
 }
 openRelation(item:ReasonItem,slug:string):void{void this.router.navigate(['/app',slug],{queryParams:{reasonId:item.id}});}
 exportExcel():void{
 if(!this.canExport())return;this.exporting.set(true);
 this.service.export(this.query()).pipe(finalize(()=>this.exporting.set(false))).subscribe({
 next:blob=>{const u=URL.createObjectURL(blob),a=document.createElement('a');a.href=u;
 a.download='Muayene_Sebebi_Tanimlari.xlsx';a.click();setTimeout(()=>URL.revokeObjectURL(u),1000);},
 error:e=>this.notice(e)});
 }
 private errorText(error:unknown):string {
 const e=error as HttpErrorResponse;
 return e.error?.detail||e.error?.title||e.message||'İşlem gerçekleştirilemedi.';
 }
 private notice(error:unknown):void{this.toast.add({severity:'error',summary:'İşlem başarısız',detail:this.errorText(error),life:6500});}
}

import {Component,computed,inject,OnInit,signal} from '@angular/core';
import {HttpErrorResponse} from '@angular/common/http';
import {ActivatedRoute,RouterLink} from '@angular/router';
import {FormBuilder,ReactiveFormsModule,Validators} from '@angular/forms';
import {finalize} from 'rxjs';
import {ConfirmationService,MessageService} from 'primeng/api';
import {AutoCompleteModule,AutoCompleteCompleteEvent} from 'primeng/autocomplete';
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
import {EquivalentProcedureService} from '../services/equivalent-procedure.service';
import {EquivalentProcedureItem,EquivalentProcedureOption,EquivalentProcedureQuery,
 EquivalentProcedureFields} from '../models/equivalent-procedure.model';
/** İki farklı tetkik arasında çift yönlü eşdeğerlik tanımlama ekranı. */
@Component({
 selector:'app-equivalent-procedure-list',standalone:true,
 imports:[RouterLink,ReactiveFormsModule,AutoCompleteModule,ButtonModule,CheckboxModule,
  ConfirmDialogModule,DialogModule,InputTextModule,TableModule,TagModule,ToastModule,AuditHistoryComponent],
 providers:[ConfirmationService,MessageService],
 templateUrl:'./equivalent-procedure-list.component.html',
 styleUrl:'./equivalent-procedure-list.component.scss'
})
export class EquivalentProcedureListComponent implements OnInit {
 private readonly api=inject(EquivalentProcedureService);
 private readonly route=inject(ActivatedRoute);
 private readonly fb=inject(FormBuilder);
 private readonly permissions=inject(PermissionService);
 private readonly confirmation=inject(ConfirmationService);
 private readonly message=inject(MessageService);
 readonly canCreate=computed(()=>this.permissions.has(PermissionCodes.EquivalentProcedures.Create));
 readonly canUpdate=computed(()=>this.permissions.has(PermissionCodes.EquivalentProcedures.Update));
 readonly canDelete=computed(()=>this.permissions.has(PermissionCodes.EquivalentProcedures.Delete));
 readonly canExport=computed(()=>this.permissions.has(PermissionCodes.EquivalentProcedures.Query));
 readonly canAudit=computed(()=>this.permissions.has(PermissionCodes.AuditLogs.View));
 readonly records=signal<EquivalentProcedureItem[]>([]);
 readonly page=signal(1);readonly pageSize=signal(20);readonly total=signal(0);
 readonly maxPage=computed(()=>Math.max(1,Math.ceil(this.total()/this.pageSize())));
 readonly loading=signal(false);readonly saving=signal(false);readonly exporting=signal(false);
 readonly dialogVisible=signal(false);readonly editing=signal<EquivalentProcedureItem|null>(null);
 readonly auditRecord=signal<EquivalentProcedureItem|null>(null);
 readonly errorMessage=signal<string|null>(null);readonly formError=signal<string|null>(null);
 readonly primaryOptions=signal<EquivalentProcedureOption[]>([]);
 readonly equivalentOptions=signal<EquivalentProcedureOption[]>([]);
 readonly filterForm=this.fb.nonNullable.group({search:'',includeInactive:false});
 readonly form=this.fb.group({
  procedure:this.fb.control<EquivalentProcedureOption|null>(null,Validators.required),
  equivalent:this.fb.control<EquivalentProcedureOption|null>(null,Validators.required)
 });
 readonly procedureFilterId=this.route.snapshot.queryParamMap.get('procedureId');
 private listRequest=0;private editRequest=0;private primaryRequest=0;private equivalentRequest=0;
 ngOnInit():void{this.load();}
 private query():EquivalentProcedureQuery{
  const v=this.filterForm.getRawValue();
  return {search:v.search,procedureId:this.procedureFilterId,includeInactive:v.includeInactive,
   isActive:null,pageNumber:this.page(),pageSize:this.pageSize()};
 }
 load():void {
  const request=++this.listRequest;this.loading.set(true);
  this.api.list(this.query()).pipe(finalize(()=>{if(request===this.listRequest)this.loading.set(false);})).subscribe({
   next:r=>{if(request!==this.listRequest)return;this.records.set(r.items);this.total.set(r.totalCount);},
   error:e=>{if(request!==this.listRequest)return;this.records.set([]);this.total.set(0);this.notice(e);}
  });
 }
 search():void{this.page.set(1);this.load();}
 clear():void{this.filterForm.reset({search:'',includeInactive:false});this.search();}
 goToPage(p:number):void{if(p<1||p>this.maxPage()||this.loading())return;this.page.set(p);this.load();}
 setPageSize(e:Event):void{this.pageSize.set(Number((e.target as HTMLSelectElement).value));this.search();}
 /** PrimeNG AutoComplete dropdown paneline sunucuda aranan 60 tetkik getirilir. */
 loadPrimary(e:AutoCompleteCompleteEvent):void{
  const req=++this.primaryRequest;
  this.api.options(e.query||'',this.form.controls.procedure.value?.id).subscribe({
   next:o=>{if(req===this.primaryRequest)this.primaryOptions.set(o);},
   error:err=>{if(req===this.primaryRequest)this.notice(err);}
  });
 }
 loadEquivalent(e:AutoCompleteCompleteEvent):void{
  const req=++this.equivalentRequest;
  this.api.options(e.query||'',this.form.controls.equivalent.value?.id).subscribe({
   next:o=>{if(req===this.equivalentRequest)this.equivalentOptions.set(o);},
   error:err=>{if(req===this.equivalentRequest)this.notice(err);}
  });
 }
 private option(id:string,code:string,name:string):EquivalentProcedureOption{
  return {id,code,name,isActive:true,label:code+' — '+name};
 }
 openCreate():void{
  if(!this.canCreate())return;
  ++this.editRequest;this.editing.set(null);this.formError.set(null);
  this.form.reset({procedure:null,equivalent:null});
  this.primaryOptions.set([]);this.equivalentOptions.set([]);
  this.dialogVisible.set(true);
  if(this.procedureFilterId){
   const request=this.editRequest;
   this.api.options('',this.procedureFilterId).subscribe({next:opts=>{
     const selected=opts.find(x=>x.id===this.procedureFilterId);
     if(request===this.editRequest&&selected&&!this.editing()&&this.dialogVisible())
       this.form.controls.procedure.setValue(selected);
   },error:e=>this.notice(e)});
  }
 }
 openEdit(row:EquivalentProcedureItem):void{
  if(!this.canUpdate())return;
  const request=++this.editRequest;
  this.api.detail(row.id).subscribe({next:item=>{
   if(request!==this.editRequest)return;
   this.editing.set(item);this.formError.set(null);
   this.form.reset({procedure:this.option(item.procedureDefId,item.procedureCode,item.procedureName),
    equivalent:this.option(item.equivalentProcedureId,item.equivalentProcedureCode,item.equivalentProcedureName)});
   this.dialogVisible.set(true);
  },error:e=>this.notice(e)});
 }
 close():void{if(this.saving())return;this.dialogVisible.set(false);++this.editRequest;}
 save():void{
  if(this.saving()||(this.editing()?!this.canUpdate():!this.canCreate()))return;
  this.formError.set(null);
  const p=this.form.controls.procedure.value,q=this.form.controls.equivalent.value;
  if(this.form.invalid||!p?.id||!q?.id){this.form.markAllAsTouched();
    this.formError.set('Her iki tetkiki açılır seçim panelinden seçiniz.');return;}
  if(p.id===q.id){this.formError.set('Bir tetkik kendisiyle eşleştirilemez.');return;}
  const fields:EquivalentProcedureFields={procedureDefId:p.id,equivalentProcedureId:q.id};
  const current=this.editing();
  const action=current?this.api.update({...fields,id:current.id,version:current.version}):this.api.create(fields);
  this.saving.set(true);
  action.pipe(finalize(()=>this.saving.set(false))).subscribe({
   next:()=>{this.dialogVisible.set(false);this.message.add({severity:'success',summary:'Eşleştirme kaydedildi'});this.load();},
   error:e=>{this.formError.set(this.errorText(e));this.notice(e);}
  });
 }
 status(row:EquivalentProcedureItem):void{
  if(!this.canUpdate())return;
  this.confirmation.confirm({header:'Durum Değiştir',message:'Eşdeğer tetkik eşleştirmesinin durumu değiştirilsin mi?',
   acceptLabel:'Onayla',rejectLabel:'Vazgeç',
   accept:()=>this.api.status(row).subscribe({next:()=>this.load(),error:e=>this.notice(e)})});
 }
 remove(row:EquivalentProcedureItem):void{
  if(!this.canDelete())return;
  this.confirmation.confirm({header:'Silme Onayı',message:'Bu eşdeğer tetkik eşleştirmesi silinsin mi?',
   acceptLabel:'Sil',rejectLabel:'Vazgeç',
   accept:()=>this.api.delete(row).subscribe({next:()=>this.load(),error:e=>this.notice(e)})});
 }
 excel():void{
  if(!this.canExport())return;this.exporting.set(true);
  this.api.export(this.query()).pipe(finalize(()=>this.exporting.set(false))).subscribe({
    next:blob=>{const url=URL.createObjectURL(blob),a=document.createElement('a');
     a.href=url;a.download='esdeger-tetkikler.xlsx';a.click();setTimeout(()=>URL.revokeObjectURL(url),1000);},
    error:e=>this.notice(e)});
 }
 private errorText(e:unknown):string{
  const err=e as HttpErrorResponse;
  return err.error?.detail||err.error?.title||err.message||'İşlem gerçekleştirilemedi.';
 }
 private notice(e:unknown):void{
  const detail=this.errorText(e);this.errorMessage.set(detail);
  this.message.add({severity:'error',summary:'İşlem başarısız',detail,life:7000});
 }
}

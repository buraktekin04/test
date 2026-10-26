import {Component,computed,inject,OnInit,signal} from '@angular/core';
import {HttpErrorResponse} from '@angular/common/http';
import {FormBuilder,ReactiveFormsModule,Validators} from '@angular/forms';
import {RouterLink} from '@angular/router';
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
import {SyyDiagnosisDecisionService} from '../services/syy-diagnosis-decision.service';
import {SyyDiagnosisDecisionItem,SyyDiagnosisDecisionOptions,SyyDiagnosisDecisionQuery,
 SyyDiagnosisDecisionFields} from '../models/syy-diagnosis-decision.model';
/** Eski SYYDiagnosisDecision modeline uygun, ayrı CRUD tanımlama ekranı. */
@Component({
 selector:'app-syy-diagnosis-decision-list',standalone:true,
 imports:[RouterLink,ReactiveFormsModule,ButtonModule,CheckboxModule,ConfirmDialogModule,
  DialogModule,InputTextModule,TableModule,TagModule,ToastModule,AuditHistoryComponent],
 providers:[ConfirmationService,MessageService],
 templateUrl:'./syy-diagnosis-decision-list.component.html',
 styleUrl:'./syy-diagnosis-decision-list.component.scss'
})
export class SyyDiagnosisDecisionListComponent implements OnInit {
 private readonly api=inject(SyyDiagnosisDecisionService);
 private readonly permissions=inject(PermissionService);
 private readonly fb=inject(FormBuilder);
 private readonly confirmation=inject(ConfirmationService);
 private readonly messages=inject(MessageService);
 readonly canCreate=computed(()=>this.permissions.has(PermissionCodes.SYYDiagnosisDecisions.Create));
 readonly canUpdate=computed(()=>this.permissions.has(PermissionCodes.SYYDiagnosisDecisions.Update));
 readonly canDelete=computed(()=>this.permissions.has(PermissionCodes.SYYDiagnosisDecisions.Delete));
 readonly canQuery=computed(()=>this.permissions.has(PermissionCodes.SYYDiagnosisDecisions.Query));
 readonly canAudit=computed(()=>this.permissions.has(PermissionCodes.AuditLogs.View));
 readonly records=signal<SyyDiagnosisDecisionItem[]>([]);
 readonly optionsData=signal<SyyDiagnosisDecisionOptions>({syyDiagnoses:[],decisions:[],admissionTypes:[],militaryClasses:[]});
 readonly loading=signal(false);readonly saving=signal(false);readonly exporting=signal(false);
 readonly page=signal(1);readonly pageSize=signal(20);readonly total=signal(0);
 readonly maxPage=computed(()=>Math.max(1,Math.ceil(this.total()/this.pageSize())));
 readonly showForm=signal(false);readonly editing=signal<SyyDiagnosisDecisionItem|null>(null);
 readonly auditItem=signal<SyyDiagnosisDecisionItem|null>(null);readonly formError=signal<string|null>(null);
 readonly searchForm=this.fb.nonNullable.group({search:'',includeInactive:false});
 readonly form=this.fb.group({
   syyDiagnosisDefId:this.fb.nonNullable.control('',Validators.required),
   hcDecisionDefId:this.fb.nonNullable.control('',Validators.required),
   hcAdmissionTypeDefId:this.fb.nonNullable.control('',Validators.required),
   militaryClassDefId:this.fb.nonNullable.control(''),
   amount:this.fb.nonNullable.control(0,[Validators.required,Validators.min(0)])
 });
 private requestId=0;private editRequestId=0;
 ngOnInit():void{this.load();}
 private query():SyyDiagnosisDecisionQuery{
  const q=this.searchForm.getRawValue();
  return {search:q.search,includeInactive:q.includeInactive,isActive:null,pageNumber:this.page(),
   pageSize:this.pageSize(),syyDiagnosisDefId:null,hcDecisionDefId:null,hcAdmissionTypeDefId:null,militaryClassDefId:null};
 }
 load():void {
  const id=++this.requestId;this.loading.set(true);
  this.api.list(this.query()).pipe(finalize(()=>{if(id===this.requestId)this.loading.set(false);})).subscribe({
   next:r=>{if(id!==this.requestId)return;this.records.set(r.items);this.total.set(r.totalCount);},
   error:e=>{if(id!==this.requestId)return;this.records.set([]);this.total.set(0);this.notice(e);}
  });
 }
 search():void{this.page.set(1);this.load();}
 clear():void{this.searchForm.reset({search:'',includeInactive:false});this.search();}
 toPage(p:number):void{if(p<1||p>this.maxPage()||this.loading())return;this.page.set(p);this.load();}
 changeSize(event:Event):void{this.pageSize.set(Number((event.target as HTMLSelectElement).value));this.search();}
 private options(row?:SyyDiagnosisDecisionItem):void{
  this.api.options(row).subscribe({next:r=>this.optionsData.set(r),error:e=>this.notice(e)});
 }
 openCreate():void{
  if(!this.canCreate())return; ++this.editRequestId;
  this.editing.set(null);this.formError.set(null);
  this.form.reset({syyDiagnosisDefId:'',hcDecisionDefId:'',hcAdmissionTypeDefId:'',militaryClassDefId:'',amount:0});
  this.options();this.showForm.set(true);
 }
 openEdit(row:SyyDiagnosisDecisionItem):void{
  if(!this.canUpdate())return;const request=++this.editRequestId;
  this.api.detail(row.id).subscribe({next:item=>{
   if(request!==this.editRequestId)return;
   this.editing.set(item);this.formError.set(null);
   this.form.reset({syyDiagnosisDefId:item.syyDiagnosisDefId,hcDecisionDefId:item.hcDecisionDefId,
    hcAdmissionTypeDefId:item.hcAdmissionTypeDefId,militaryClassDefId:item.militaryClassDefId??'',amount:item.amount});
   this.options(item);this.showForm.set(true);
  },error:e=>this.notice(e)});
 }
 save():void {
  if(this.saving()||(this.editing()?!this.canUpdate():!this.canCreate()))return;
  this.formError.set(null);const v=this.form.getRawValue();
  if(this.form.invalid||!Number.isInteger(v.amount)||v.amount<0){
   this.form.markAllAsTouched();this.formError.set('Tanı, karar ve başvuru grubu zorunludur. Adet negatif olmayan tam sayı olmalıdır.');return;
  }
  const fields:SyyDiagnosisDecisionFields={syyDiagnosisDefId:v.syyDiagnosisDefId,hcDecisionDefId:v.hcDecisionDefId,
   hcAdmissionTypeDefId:v.hcAdmissionTypeDefId,militaryClassDefId:v.militaryClassDefId||null,amount:v.amount};
  const row=this.editing();
  const request=row?this.api.update({...fields,id:row.id,version:row.version}):this.api.create(fields);
  this.saving.set(true);
  request.pipe(finalize(()=>this.saving.set(false))).subscribe({
   next:()=>{this.showForm.set(false);this.messages.add({severity:'success',summary:'Kaydedildi'});this.load();},
   error:e=>{this.formError.set(this.errorText(e));this.notice(e);}
  });
 }
 changeStatus(row:SyyDiagnosisDecisionItem):void {
  if(!this.canUpdate())return;
  this.confirmation.confirm({header:'Durum Değişikliği',message:'İstatistik tanımının durumu değiştirilsin mi?',
   acceptLabel:'Onayla',rejectLabel:'Vazgeç',
   accept:()=>this.api.changeStatus(row).subscribe({next:()=>this.load(),error:e=>this.notice(e)})});
 }
 remove(row:SyyDiagnosisDecisionItem):void {
  if(!this.canDelete())return;
  this.confirmation.confirm({header:'Silme Onayı',message:'Bu istatistik tanımı silinsin mi?',
   acceptLabel:'Sil',rejectLabel:'Vazgeç',
   accept:()=>this.api.delete(row).subscribe({next:()=>this.load(),error:e=>this.notice(e)})});
 }
 exportExcel():void {
  if(!this.canQuery())return;this.exporting.set(true);
  this.api.export(this.query()).pipe(finalize(()=>this.exporting.set(false))).subscribe({
   next:blob=>{const url=URL.createObjectURL(blob),a=document.createElement('a');a.href=url;
    a.download='syy-tani-karar-istatistigi.xlsx';a.click();setTimeout(()=>URL.revokeObjectURL(url),1000);},
   error:e=>this.notice(e)});
 }
 private errorText(error:unknown):string{
  const e=error as HttpErrorResponse;return e.error?.detail||e.error?.title||e.message||'İşlem gerçekleştirilemedi.';
 }
 private notice(error:unknown):void{
  this.messages.add({severity:'error',summary:'İşlem başarısız',detail:this.errorText(error),life:7000});
 }
}

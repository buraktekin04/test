import {Component,computed,inject,OnInit,signal} from '@angular/core';
import {HttpErrorResponse} from '@angular/common/http';
import {ActivatedRoute,RouterLink} from '@angular/router';
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
import {Icd10DiagnosticService} from '../services/icd10-diagnostic.service';
import {Icd10DiagnosticItem,Icd10DiagnosticOptions,Icd10DiagnosticQuery} from '../models/icd10-diagnostic.model';
/** ICD10 SYY Tanı İstatistik Tanımları — bağımsız Angular sayfası, üç FK + Amount. */
@Component({
 selector:'app-icd10-diagnostic-list',standalone:true,
 imports:[RouterLink,ReactiveFormsModule,ButtonModule,CheckboxModule,ConfirmDialogModule,
 DialogModule,InputTextModule,TableModule,TagModule,ToastModule,AuditHistoryComponent],
 providers:[ConfirmationService,MessageService],
 templateUrl:'./icd10-diagnostic-list.component.html',
 styleUrl:'./icd10-diagnostic-list.component.scss'
})
export class Icd10DiagnosticListComponent implements OnInit {
 private readonly api=inject(Icd10DiagnosticService);
 private readonly route=inject(ActivatedRoute);
 private readonly fb=inject(FormBuilder);
 private readonly permissions=inject(PermissionService);
 private readonly confirm=inject(ConfirmationService);
 private readonly toast=inject(MessageService);
 readonly presetICD10DefId=this.route.snapshot.queryParamMap.get('icd10DefId');
 readonly canCreate=computed(()=>this.permissions.has(PermissionCodes.ICD10Diagnostics.Create));
 readonly canUpdate=computed(()=>this.permissions.has(PermissionCodes.ICD10Diagnostics.Update));
 readonly canDelete=computed(()=>this.permissions.has(PermissionCodes.ICD10Diagnostics.Delete));
 readonly canExport=computed(()=>this.permissions.has(PermissionCodes.ICD10Diagnostics.Query));
 readonly canAudit=computed(()=>this.permissions.has(PermissionCodes.AuditLogs.View));
 readonly items=signal<Icd10DiagnosticItem[]>([]);
 readonly optionsData=signal<Icd10DiagnosticOptions>({icd10s:[],syyDiagnoses:[],admissionTypes:[]});
 readonly page=signal(1);readonly pageSize=signal(20);readonly total=signal(0);
 readonly maxPage=computed(()=>Math.max(1,Math.ceil(this.total()/this.pageSize())));
 readonly loading=signal(false);readonly saving=signal(false);readonly exporting=signal(false);
 readonly showForm=signal(false);readonly editing=signal<Icd10DiagnosticItem|null>(null);
 readonly auditItem=signal<Icd10DiagnosticItem|null>(null);readonly formError=signal<string|null>(null);
 readonly showFilters=signal(false);
 readonly searchForm=this.fb.nonNullable.group({search:'',includeInactive:false});
 readonly activeFilter=this.fb.control<boolean|null>(null);
 readonly form=this.fb.group({
  icd10DefId:this.fb.nonNullable.control('',Validators.required),
  syyDiagnosisDefId:this.fb.nonNullable.control('',Validators.required),
  hcAdmissionTypeDefId:this.fb.nonNullable.control('',Validators.required),
  amount:this.fb.nonNullable.control(0,[Validators.required,Validators.min(0)])
 });
 private requestId=0;private editId=0;
 ngOnInit(){this.load();}
 private query():Icd10DiagnosticQuery{
  const f=this.searchForm.getRawValue();
  return {search:f.search,includeInactive:f.includeInactive||this.activeFilter.value===false,
   isActive:this.activeFilter.value,pageNumber:this.page(),pageSize:this.pageSize(),
   icd10DefId:this.presetICD10DefId,syyDiagnosisDefId:null,hcAdmissionTypeDefId:null};
 }
 load(){
  const id=++this.requestId;this.loading.set(true);
  this.api.list(this.query()).pipe(finalize(()=>{if(id===this.requestId)this.loading.set(false);}))
   .subscribe({next:r=>{if(id!==this.requestId)return;this.items.set(r.items);this.total.set(r.totalCount);},
   error:e=>{if(id!==this.requestId)return;this.items.set([]);this.total.set(0);this.notify(e);}});
 }
 search(){this.page.set(1);this.load();}
 clear(){this.searchForm.reset({search:'',includeInactive:false});this.activeFilter.setValue(null);this.search();}
 toPage(n:number){if(n<1||n>this.maxPage()||this.loading())return;this.page.set(n);this.load();}
 setPageSize(e:Event){this.pageSize.set(Number((e.target as HTMLSelectElement).value));this.search();}
 private loadOptions(row?:Icd10DiagnosticItem){
  this.api.options(row).subscribe({next:o=>this.optionsData.set(o),error:e=>this.notify(e)});
 }
 openCreate(){
  if(!this.canCreate())return;
  ++this.editId;this.editing.set(null);this.formError.set(null);
  this.form.reset({icd10DefId:this.presetICD10DefId||'',syyDiagnosisDefId:'',hcAdmissionTypeDefId:'',amount:0});
  this.loadOptions();this.showForm.set(true);
 }
 openEdit(row:Icd10DiagnosticItem){
  if(!this.canUpdate())return;
  const id=++this.editId;
  this.api.detail(row.id).subscribe({next:item=>{
   if(id!==this.editId)return;
   this.editing.set(item);this.formError.set(null);
   this.form.reset({icd10DefId:item.icd10DefId,syyDiagnosisDefId:item.syyDiagnosisDefId,
    hcAdmissionTypeDefId:item.hcAdmissionTypeDefId,amount:item.amount});
   this.loadOptions(item);this.showForm.set(true);
  },error:e=>this.notify(e)});
 }
 save(){
  if(this.saving()||(this.editing()?!this.canUpdate():!this.canCreate()))return;
  this.formError.set(null);const f=this.form.getRawValue();
  if(this.form.invalid||!f.icd10DefId||!f.syyDiagnosisDefId||!f.hcAdmissionTypeDefId||
    !Number.isInteger(f.amount)||f.amount<0){
   this.form.markAllAsTouched();this.formError.set('Üç tanımı ve sıfır veya üzeri tam sayı adedi giriniz.');return;
  }
  const payload:Record<string,unknown>={
   icd10DefId:f.icd10DefId,syyDiagnosisDefId:f.syyDiagnosisDefId,
   hcAdmissionTypeDefId:f.hcAdmissionTypeDefId,amount:f.amount
  };
  const current=this.editing();
  const req=current?this.api.update(current.id,{...payload,id:current.id,version:current.version}):
   this.api.create(payload);
  this.saving.set(true);
  req.pipe(finalize(()=>this.saving.set(false))).subscribe({
   next:()=>{this.showForm.set(false);this.toast.add({severity:'success',summary:'Kaydedildi'});this.load();},
   error:e=>{this.formError.set(this.errorText(e));this.notify(e);}
  });
 }
 status(row:Icd10DiagnosticItem){
  if(!this.canUpdate())return;
  this.confirm.confirm({header:'Durum Değişikliği',message:'Bu istatistik tanımının durumu değiştirilsin mi?',
   acceptLabel:'Onayla',rejectLabel:'Vazgeç',accept:()=>this.api.status(row)
    .subscribe({next:()=>this.load(),error:e=>this.notify(e)})});
 }
 remove(row:Icd10DiagnosticItem){
  if(!this.canDelete())return;
  this.confirm.confirm({header:'Silme Onayı',message:'Seçilen istatistik tanımı silinsin mi?',
   acceptLabel:'Sil',rejectLabel:'Vazgeç',accept:()=>this.api.delete(row)
    .subscribe({next:()=>this.load(),error:e=>this.notify(e)})});
 }
 exportExcel(){
  if(!this.canExport())return;this.exporting.set(true);
  this.api.export(this.query()).pipe(finalize(()=>this.exporting.set(false)))
   .subscribe({next:blob=>{const u=URL.createObjectURL(blob),a=document.createElement('a');
    a.href=u;a.download='icd10-syy-istatistik.xlsx';a.click();setTimeout(()=>URL.revokeObjectURL(u),1000);},
    error:e=>this.notify(e)});
 }
 private errorText(e:unknown){const x=e as HttpErrorResponse;return x.error?.detail||x.error?.title||x.message||'İşlem başarısız.';}
 private notify(e:unknown){this.toast.add({severity:'error',summary:'İşlem başarısız',detail:this.errorText(e),life:7000});}
}

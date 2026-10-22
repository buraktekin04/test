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
import {SyyDiagnosisRequiredService} from '../services/syy-diagnosis-required.service';
import {SyyRequiredItem,SyyRequiredOption,SyyRequiredQuery} from '../models/syy-diagnosis-required.model';
/** SYY Onay Gerektiren Maddeler: tek ekran, kural ve alt maddeler için ayrı DB tabloları. */
@Component({
 selector:'app-syy-diagnosis-required-list',standalone:true,
 imports:[RouterLink,ReactiveFormsModule,ButtonModule,CheckboxModule,ConfirmDialogModule,
 DialogModule,InputTextModule,TableModule,TagModule,ToastModule,AuditHistoryComponent],
 providers:[ConfirmationService,MessageService],
 templateUrl:'./syy-diagnosis-required-list.component.html',
 styleUrl:'./syy-diagnosis-required-list.component.scss'
})
export class SyyDiagnosisRequiredListComponent implements OnInit {
 private readonly api=inject(SyyDiagnosisRequiredService);
 private readonly route=inject(ActivatedRoute);
 private readonly fb=inject(FormBuilder);
 private readonly permissions=inject(PermissionService);
 private readonly confirm=inject(ConfirmationService);
 private readonly toast=inject(MessageService);
 readonly presetDiagnosisId=this.route.snapshot.queryParamMap.get('syyDiagnosisId');
 readonly canCreate=computed(()=>this.permissions.has(PermissionCodes.SYYDiagnosisRequireds.Create));
 readonly canUpdate=computed(()=>this.permissions.has(PermissionCodes.SYYDiagnosisRequireds.Update));
 readonly canDelete=computed(()=>this.permissions.has(PermissionCodes.SYYDiagnosisRequireds.Delete));
 readonly canExport=computed(()=>this.permissions.has(PermissionCodes.SYYDiagnosisRequireds.Query));
 readonly canAudit=computed(()=>this.permissions.has(PermissionCodes.AuditLogs.View));
 readonly records=signal<SyyRequiredItem[]>([]);readonly options=signal<SyyRequiredOption[]>([]);
 readonly total=signal(0);readonly page=signal(1);readonly pageSize=signal(20);
 readonly maxPage=computed(()=>Math.max(1,Math.ceil(this.total()/this.pageSize())));
 readonly loading=signal(false);readonly saving=signal(false);readonly exporting=signal(false);
 readonly showForm=signal(false);readonly editing=signal<SyyRequiredItem|null>(null);
 readonly audit=signal<SyyRequiredItem|null>(null);readonly formError=signal<string|null>(null);
 readonly showFilters=signal(false);
 readonly searchForm=this.fb.nonNullable.group({search:'',includeInactive:false});
 readonly statusFilter=this.fb.control<boolean|null>(null);
 readonly form=this.fb.group({
  syyDiagnosisDefId:this.fb.nonNullable.control('',Validators.required),
  mustAll:this.fb.control<boolean|null>(false),
  mustOneOf:this.fb.control<boolean|null>(false)
 });
 /** Sıralı açıklamalar; formun içinde eklenir/çıkarılır, ayrı tanım ekranı değildir. */
 readonly descriptions=this.fb.nonNullable.array<string>([]);
 private rid=0;
 ngOnInit(){this.load();this.loadOptions();}
 private query():SyyRequiredQuery{
  const q=this.searchForm.getRawValue();
  return {search:q.search,syyDiagnosisDefId:this.presetDiagnosisId,
   includeInactive:q.includeInactive||this.statusFilter.value===false,
   isActive:this.statusFilter.value,pageNumber:this.page(),pageSize:this.pageSize()};
 }
 load(){
  const id=++this.rid;this.loading.set(true);
  this.api.list(this.query()).pipe(finalize(()=>{if(id===this.rid)this.loading.set(false);}))
   .subscribe({next:r=>{if(id!==this.rid)return;this.records.set(r.items);this.total.set(r.totalCount);},
   error:e=>{if(id!==this.rid)return;this.records.set([]);this.total.set(0);this.notice(e);}});
 }
 search(){this.page.set(1);this.load();}
 clear(){this.searchForm.reset({search:'',includeInactive:false});this.statusFilter.setValue(null);this.search();}
 changePage(n:number){if(n<1||n>this.maxPage()||this.loading())return;this.page.set(n);this.load();}
 changeSize(e:Event){this.pageSize.set(Number((e.target as HTMLSelectElement).value));this.search();}
 loadOptions(selectedId?:string){this.api.options(selectedId).subscribe({
  next:r=>this.options.set(r),error:e=>this.notice(e)});}
 addDescription(text=''){
  if(this.descriptions.length>=50)return;
  this.descriptions.push(this.fb.nonNullable.control(text,[Validators.required,Validators.maxLength(1500)]));
 }
 removeDescription(index:number){if(this.descriptions.length>1)this.descriptions.removeAt(index);}
 private resetDescriptions(items:string[]){
  this.descriptions.clear();
  for(const value of items.length?items:[''])this.addDescription(value);
 }
 create(){
  if(!this.canCreate())return;this.editing.set(null);this.formError.set(null);
  this.form.reset({syyDiagnosisDefId:this.presetDiagnosisId??'',mustAll:false,mustOneOf:false});
  this.resetDescriptions(['']);this.loadOptions(this.presetDiagnosisId??undefined);this.showForm.set(true);
 }
 edit(row:SyyRequiredItem){
  if(!this.canUpdate())return;
  this.api.detail(row.id).subscribe({next:x=>{
    this.editing.set(x);this.formError.set(null);
    this.form.reset({syyDiagnosisDefId:x.syyDiagnosisDefId,mustAll:x.mustAll,mustOneOf:x.mustOneOf});
    this.resetDescriptions(x.details.map(d=>d.description));
    this.loadOptions(x.syyDiagnosisDefId);this.showForm.set(true);
  },error:e=>this.notice(e)});
 }
 save(){
  if(this.saving()||(this.editing()?!this.canUpdate():!this.canCreate()))return;
  this.formError.set(null);
  const f=this.form.getRawValue(),items=this.descriptions.getRawValue().map(x=>x.trim());
  if(this.form.invalid||this.descriptions.invalid||items.length===0||items.some(x=>!x)){
   this.form.markAllAsTouched();this.descriptions.markAllAsTouched();
   this.formError.set('Tanıyı seçiniz ve en az bir madde açıklaması yazınız.');return;
  }
  if(f.mustAll===true&&f.mustOneOf===true){
   this.formError.set('Tüm maddeler ve en az bir madde koşulları birlikte seçilemez.');return;
  }
  const fields:Record<string,unknown>={syyDiagnosisDefId:f.syyDiagnosisDefId,
   mustAll:f.mustAll,mustOneOf:f.mustOneOf,descriptions:items};
  const old=this.editing();
  const req=old?this.api.update(old.id,{...fields,id:old.id,version:old.version}):this.api.create(fields);
  this.saving.set(true);
  req.pipe(finalize(()=>this.saving.set(false))).subscribe({next:()=>{
   this.showForm.set(false);this.load();this.toast.add({severity:'success',summary:'Kaydedildi'});
  },error:e=>{this.formError.set(this.errorText(e));this.notice(e);}});
 }
 toggle(item:SyyRequiredItem){
  if(!this.canUpdate())return;
  this.confirm.confirm({header:'Durum Onayı',message:'Kuralın durumu değiştirilsin mi?',
   acceptLabel:'Onayla',rejectLabel:'Vazgeç',accept:()=>this.api.status(item)
    .subscribe({next:()=>this.load(),error:e=>this.notice(e)})});
 }
 delete(item:SyyRequiredItem){
  if(!this.canDelete())return;
  this.confirm.confirm({header:'Silme Onayı',message:'Kural ve tüm alt maddeler silinsin mi?',
   acceptLabel:'Sil',rejectLabel:'Vazgeç',accept:()=>this.api.delete(item)
    .subscribe({next:()=>this.load(),error:e=>this.notice(e)})});
 }
 excel(){
  if(!this.canExport())return;this.exporting.set(true);
  this.api.export(this.query()).pipe(finalize(()=>this.exporting.set(false)))
   .subscribe({next:blob=>{const url=URL.createObjectURL(blob),a=document.createElement('a');
    a.href=url;a.download='syy-onay-maddeler.xlsx';a.click();setTimeout(()=>URL.revokeObjectURL(url),1000);},
    error:e=>this.notice(e)});
 }
 private errorText(e:unknown){const x=e as HttpErrorResponse;return x.error?.detail||x.error?.title||x.message||'İşlem başarısız.';}
 private notice(e:unknown){this.toast.add({severity:'error',summary:'İşlem başarısız',detail:this.errorText(e),life:7000});}
}

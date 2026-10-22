import {Component,computed,inject,OnInit,signal} from '@angular/core';
import {DatePipe} from '@angular/common';
import {HttpErrorResponse} from '@angular/common/http';
import {RouterLink} from '@angular/router';
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
import {PermissionCodes} from '../../../core/permissions/constants/permission-codes';
import {PermissionService} from '../../../core/permissions/services/permission.service';
import {AuditHistoryComponent} from '../../../shared/audit-history/audit-history.component';
import {ValidityPeriodUtils} from '../../../core/utils/validity-period.utils';
import {SyyDiagnosisService} from '../services/syy-diagnosis.service';
import {SyyDiagnosisItem,DefinitionQuery,DiagnosisOptions} from '../models/syy-diagnosis.model';
const emptyOptions:DiagnosisOptions={types:[],slices:[],parents:[]};
/** SYY Tanı Listesi (M/D/F) — bağımsız ekran, kendi formu/template'i ve permission'ları. */
@Component({
 selector:'app-syy-diagnosis-list',standalone:true,
 imports:[DatePipe,RouterLink,ReactiveFormsModule,ButtonModule,CheckboxModule,ConfirmDialogModule,
 DialogModule,InputTextModule,TableModule,TagModule,ToastModule,AuditHistoryComponent],
 providers:[ConfirmationService,MessageService],
 templateUrl:'./syy-diagnosis-list.component.html',
 styleUrl:'./syy-diagnosis-list.component.scss'
})
export class SyyDiagnosisListComponent implements OnInit {
 private readonly api=inject(SyyDiagnosisService);
 private readonly fb=inject(FormBuilder);
 private readonly permissions=inject(PermissionService);
 private readonly confirm=inject(ConfirmationService);
 private readonly message=inject(MessageService);
 readonly config = { module:'SYYDiagnoses' as const, path:'syy-diagnoses', title:'SYY Tanı Listesi (M/D/F)', entity:'SYYDiagnosisDef' };
 readonly canCreate=computed(()=>this.permissions.has(PermissionCodes[this.config.module].Create));
 readonly canUpdate=computed(()=>this.permissions.has(PermissionCodes[this.config.module].Update));
 readonly canDelete=computed(()=>this.permissions.has(PermissionCodes[this.config.module].Delete));
 readonly canExport=computed(()=>this.permissions.has(PermissionCodes[this.config.module].Query));
 readonly canAudit=computed(()=>this.permissions.has(PermissionCodes.AuditLogs.View));
 readonly items=signal<SyyDiagnosisItem[]>([]);readonly total=signal(0);
 readonly page=signal(1);readonly pageSize=signal(20);
 readonly maxPage=computed(()=>Math.max(1,Math.ceil(this.total()/this.pageSize())));
 readonly loading=signal(false);readonly saving=signal(false);readonly exporting=signal(false);
 readonly dialogVisible=signal(false);readonly editing=signal<SyyDiagnosisItem|null>(null);
 readonly auditItem=signal<SyyDiagnosisItem|null>(null);
 readonly options=signal<DiagnosisOptions>(emptyOptions);
 readonly filtersVisible=signal(false);
 readonly submitted=signal(false);readonly formError=signal<string|null>(null);
 readonly searchForm=this.fb.nonNullable.group({search:'',includeInactive:false});
 readonly columnForm=this.fb.group({isActive:this.fb.control<boolean|null>(null)});
 readonly form=this.fb.group({
   code:this.fb.control<string|null>(null,[Validators.required,Validators.maxLength(100)]),
   name:this.fb.nonNullable.control('',[Validators.required,Validators.maxLength(500)]),
   validFrom:this.fb.nonNullable.control(''), validTo:this.fb.nonNullable.control(''),
   syyDiagnosisType:this.fb.control<number|null>(null,[Validators.required]),
   attachedRecNo:this.fb.control<string|null>(null),eligible:this.fb.control<boolean|null>(null),
   slice:this.fb.control<number|null>(null)
 });
 private requestId=0;private editRequestId=0;
 ngOnInit(){this.load();}
 private query():DefinitionQuery{
  const s=this.searchForm.getRawValue(),c=this.columnForm.getRawValue();
  return {search:s.search,includeInactive:s.includeInactive||c.isActive===false,
    isActive:c.isActive,pageNumber:this.page(),pageSize:this.pageSize()};
 }
 /** Sunucuda filtre/sayfalama; PrimeNG ile sadece istemcide kolon sıralama. */
 load(){
  const request=++this.requestId;this.loading.set(true);
  this.api.list(this.query()).pipe(finalize(()=>{
   if(this.requestId===request)this.loading.set(false);
  })).subscribe({next:r=>{if(this.requestId!==request)return;this.items.set(r.items);this.total.set(r.totalCount);},
   error:e=>{if(this.requestId!==request)return;this.items.set([]);this.total.set(0);this.error(e);}});
 }
 search(){this.page.set(1);this.load();}
 clear(){this.searchForm.reset({search:'',includeInactive:false});this.columnForm.reset({isActive:null});this.search();}
 toPage(n:number){if(n<1||n>this.maxPage()||this.loading())return;this.page.set(n);this.load();}
 setPageSize(event:Event){const v=Number((event.target as HTMLSelectElement).value);
 if(![10,20,50,100].includes(v))return;this.pageSize.set(v);this.search();}
 private loadOptions(item?:SyyDiagnosisItem){
  this.api.options(item?.id,item?.syyDiagnosisType,item?.slice,item?.attachedRecNo)
   .subscribe({next:x=>this.options.set(x),error:e=>this.error(e)});
 }
 openCreate(){
  if(!this.canCreate())return;
  ++this.editRequestId;this.editing.set(null);this.submitted.set(false);this.formError.set(null);
  this.form.controls.code.enable();
  this.form.reset({code:null,name:'',validFrom:'',validTo:'',syyDiagnosisType:null,
   attachedRecNo:null,eligible:false,slice:null});
  this.loadOptions();this.dialogVisible.set(true);
 }
 openEdit(item:SyyDiagnosisItem){
  if(!this.canUpdate())return;
  const id=++this.editRequestId;
  this.api.detail(item.id).subscribe({next:d=>{
    if(id!==this.editRequestId)return;
    this.editing.set(d);this.submitted.set(false);this.formError.set(null);
    this.form.reset({code:d.code,name:d.name,
      validFrom:ValidityPeriodUtils.toLocalInput(d.validFrom),
      validTo:ValidityPeriodUtils.toLocalInput(d.validTo),
      syyDiagnosisType:d.syyDiagnosisType,attachedRecNo:d.attachedRecNo,
      eligible:d.eligible===true,slice:d.slice});
    this.form.controls.code.enable();
    this.loadOptions(d);this.dialogVisible.set(true);
  },error:e=>this.error(e)});
 }
 closeDialog(){if(this.saving())return;this.dialogVisible.set(false);++this.editRequestId;}
 /** Kaynak modeldeki int? Slice / bool? Eligible değerleri aynen korur. */
 save(){
  if(this.saving()||(this.editing()?!this.canUpdate():!this.canCreate()))return;
  this.submitted.set(true);this.formError.set(null);
    const f=this.form.getRawValue();
    const code=(f.code??'').trim().toUpperCase();
    if(!code||!f.name.trim()||f.syyDiagnosisType===null||this.form.invalid){
      this.formError.set('Kod, ad ve tanı türü zorunludur.');return;
    }
    if(!ValidityPeriodUtils.hasValidRange(f.validFrom,f.validTo)){
      this.formError.set('Geçerlilik bitişi başlangıçtan sonra olmalıdır.');return;
    }
    const payload:Record<string,unknown>={
      code,name:f.name.trim(),syyDiagnosisType:f.syyDiagnosisType,
      attachedRecNo:f.attachedRecNo,
      // Nullable eski kayıtları değiştirmeden kaydederken null değerini koru.
      // Yeni kayıtta işaretlenmeyen checkbox false olarak gönderilir.
      eligible:this.editing()?.eligible===null && this.form.controls.eligible.pristine
        ? null : f.eligible===true,
      slice:f.slice,
      ...ValidityPeriodUtils.fromLocalInputs(f.validFrom,f.validTo)
    };
  const current=this.editing();
  const req=current?this.api.update(current.id,{...payload,id:current.id,version:current.version}):
    this.api.create(payload);
  this.saving.set(true);
  req.pipe(finalize(()=>this.saving.set(false))).subscribe({
    next:()=>{this.dialogVisible.set(false);this.message.add({severity:'success',
      summary:'Kaydedildi',detail:this.config.title});this.load();},
    error:e=>{this.formError.set(this.errorText(e));this.error(e);}
  });
 }

 toggleStatus(item:SyyDiagnosisItem){
  if(!this.canUpdate())return;
  this.confirm.confirm({header:'Durum Değiştir',message:'Seçilen tanımın durumu değiştirilsin mi?',
   acceptLabel:'Onayla',rejectLabel:'Vazgeç',accept:()=>{
    this.api.status(item.id,item.version,!item.isActive)
    .subscribe({next:()=>this.load(),error:e=>this.error(e)});
   }});
 }
 remove(item:SyyDiagnosisItem){
  if(!this.canDelete())return;
  this.confirm.confirm({header:'Silme Onayı',
   message:'Bu tanım silinsin mi? Bağlı tanılar varsa işlem engellenir.',
   acceptLabel:'Sil',rejectLabel:'Vazgeç',accept:()=>{
    this.api.delete(item.id,item.version)
      .subscribe({next:()=>this.load(),error:e=>this.error(e)});
   }});
 }
 exportExcel(){
  if(!this.canExport())return;this.exporting.set(true);
  this.api.export(this.query()).pipe(finalize(()=>this.exporting.set(false)))
   .subscribe({next:blob=>{const url=URL.createObjectURL(blob),el=document.createElement('a');
    el.href=url;el.download=this.config.path+'.xlsx';el.click();setTimeout(()=>URL.revokeObjectURL(url),1000);},
    error:e=>this.error(e)});
 }
 private errorText(e:unknown):string{
  const x=e as HttpErrorResponse;
  return x.error?.detail||x.error?.title||x.message||'İşlem gerçekleştirilemedi.';
 }
 private error(e:unknown){this.message.add({severity:'error',summary:'İşlem başarısız',detail:this.errorText(e),life:7000});}
}

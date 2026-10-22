import {Component,computed,inject,OnInit,signal} from '@angular/core';
import {DatePipe} from '@angular/common';
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

import {SyyDiagnosisProcedureService} from '../services/syy-diagnosis-procedure.service';
import {SyyDiagnosisProcedureItem,SyyDiagnosisProcedureItemOptions,SyyDiagnosisProcedureItemQuery} from '../models/syy-diagnosis-procedure.model';
/** SYY Gereken Tetkikler — kendi component/template/model/service dosyaları olan tanım ekranı. */
@Component({
 selector:'app-syy-diagnosis-procedure-list',standalone:true,
 imports:[DatePipe,RouterLink,ReactiveFormsModule,ButtonModule,CheckboxModule,
 ConfirmDialogModule,DialogModule,InputTextModule,TableModule,TagModule,ToastModule,AuditHistoryComponent],
 providers:[ConfirmationService,MessageService],
 templateUrl:'./syy-diagnosis-procedure-list.component.html',
 styleUrl:'./syy-diagnosis-procedure-list.component.scss'
})
export class SyyDiagnosisProcedureListComponent implements OnInit {
 private readonly service=inject(SyyDiagnosisProcedureService);
 private readonly route=inject(ActivatedRoute);
 private readonly fb=inject(FormBuilder);
 private readonly permissions=inject(PermissionService);
 private readonly confirm=inject(ConfirmationService);
 private readonly toast=inject(MessageService);
 readonly presetDiagnosisId=this.route.snapshot.queryParamMap.get('syyDiagnosisId');
 readonly canCreate=computed(()=>this.permissions.has(PermissionCodes.SYYDiagnosisProcedures.Create));
 readonly canUpdate=computed(()=>this.permissions.has(PermissionCodes.SYYDiagnosisProcedures.Update));
 readonly canDelete=computed(()=>this.permissions.has(PermissionCodes.SYYDiagnosisProcedures.Delete));
 readonly canExport=computed(()=>this.permissions.has(PermissionCodes.SYYDiagnosisProcedures.Query));
 readonly canAudit=computed(()=>this.permissions.has(PermissionCodes.AuditLogs.View));
 readonly records=signal<SyyDiagnosisProcedureItem[]>([]);
 readonly optionsData=signal<SyyDiagnosisProcedureItemOptions>({"diagnoses":[],"references":[]});
 readonly total=signal(0);readonly page=signal(1);readonly pageSize=signal(20);
 readonly maxPage=computed(()=>Math.max(1,Math.ceil(this.total()/this.pageSize())));
 readonly loading=signal(false);readonly saving=signal(false);readonly exporting=signal(false);
 readonly showForm=signal(false);readonly editing=signal<SyyDiagnosisProcedureItem|null>(null);
 readonly auditItem=signal<SyyDiagnosisProcedureItem|null>(null);readonly formError=signal<string|null>(null);
 readonly showFilters=signal(false);
 readonly filter=this.fb.nonNullable.group({search:'',includeInactive:false});
 readonly filterStatus=this.fb.control<boolean|null>(null);
 readonly form=this.fb.group({
  syyDiagnosisDefId:this.fb.nonNullable.control('',Validators.required),
  procedureDefId:this.fb.nonNullable.control('',Validators.required),
  startingAge:this.fb.nonNullable.control(0,[Validators.min(0),Validators.max(130)]),
  endingAge:this.fb.nonNullable.control(130,[Validators.min(0),Validators.max(130)]),
  gender:this.fb.nonNullable.control(0,[Validators.min(0),Validators.max(20)])
 });
 private loadId=0;
 ngOnInit(){this.reload();this.loadOptions();}
 private query():SyyDiagnosisProcedureItemQuery{
  const f=this.filter.getRawValue();
  return {search:f.search,includeInactive:f.includeInactive||this.filterStatus.value===false,
   isActive:this.filterStatus.value,syyDiagnosisDefId:this.presetDiagnosisId,
   pageNumber:this.page(),pageSize:this.pageSize()};
 }
 reload(){
  const id=++this.loadId;this.loading.set(true);
  this.service.list(this.query()).pipe(finalize(()=>{if(id===this.loadId)this.loading.set(false);}))
   .subscribe({next:r=>{if(id!==this.loadId)return;this.records.set(r.items);this.total.set(r.totalCount);},
    error:e=>{if(id!==this.loadId)return;this.records.set([]);this.total.set(0);this.error(e);}});
 }
 search(){this.page.set(1);this.reload();}
 clear(){this.filter.reset({search:'',includeInactive:false});this.filterStatus.setValue(null);this.search();}
 changePage(n:number){if(n<1||n>this.maxPage()||this.loading())return;this.page.set(n);this.reload();}
 changeSize(e:Event){this.pageSize.set(Number((e.target as HTMLSelectElement).value));this.search();}
 private loadOptions(selected?:SyyDiagnosisProcedureItem){
  this.service.options(selected).subscribe({next:x=>this.optionsData.set(x),error:e=>this.error(e)});
 }
 openCreate(){
  if(!this.canCreate())return;this.editing.set(null);this.formError.set(null);
  this.form.reset({syyDiagnosisDefId:this.presetDiagnosisId||'',procedureDefId:'',startingAge:0,endingAge:130,gender:0});this.loadOptions();this.showForm.set(true);
 }
 openEdit(row:SyyDiagnosisProcedureItem){
  if(!this.canUpdate())return;
  this.service.detail(row.id).subscribe({next:item=>{
    this.editing.set(item);this.formError.set(null);
    this.form.reset({syyDiagnosisDefId:item.syyDiagnosisDefId,procedureDefId:item.procedureDefId,startingAge:item.startingAge,endingAge:item.endingAge,gender:item.gender});this.loadOptions(item);this.showForm.set(true);
  },error:e=>this.error(e)});
 }
 save(){
  if(this.saving()||(this.editing()?!this.canUpdate():!this.canCreate()))return;
  this.formError.set(null);
  const raw=this.form.getRawValue();
  if(this.form.invalid||raw.startingAge>raw.endingAge){
   this.form.markAllAsTouched();this.formError.set('Alanları ve koşulları kontrol ediniz.');return;
  }
  const fields:Record<string,unknown>={syyDiagnosisDefId:raw.syyDiagnosisDefId,procedureDefId:raw.procedureDefId,startingAge:raw.startingAge,endingAge:raw.endingAge,gender:raw.gender};
  const item=this.editing();
  const r=item?this.service.update(item.id,{...fields,id:item.id,version:item.version}):this.service.create(fields);
  this.saving.set(true);
  r.pipe(finalize(()=>this.saving.set(false))).subscribe({
   next:()=>{this.showForm.set(false);this.reload();this.toast.add({severity:'success',summary:'Kaydedildi'});},
   error:e=>{this.formError.set(this.errorText(e));this.error(e);}
  });
 }
 status(item:SyyDiagnosisProcedureItem){
  if(!this.canUpdate())return;
  this.confirm.confirm({header:'Durum Onayı',message:'Durum değiştirilsin mi?',
   acceptLabel:'Onayla',rejectLabel:'Vazgeç',accept:()=>this.service.status(item)
   .subscribe({next:()=>this.reload(),error:e=>this.error(e)})});
 }
 delete(item:SyyDiagnosisProcedureItem){
  if(!this.canDelete())return;
  this.confirm.confirm({header:'Silme Onayı',message:'Eşleştirme silinsin mi?',
   acceptLabel:'Sil',rejectLabel:'Vazgeç',accept:()=>this.service.delete(item)
    .subscribe({next:()=>this.reload(),error:e=>this.error(e)})});
 }
 excel(){
  if(!this.canExport())return;this.exporting.set(true);
  this.service.export(this.query()).pipe(finalize(()=>this.exporting.set(false)))
   .subscribe({next:blob=>{const u=URL.createObjectURL(blob),a=document.createElement('a');
     a.href=u;a.download='syy-diagnosis-procedures.xlsx';a.click();setTimeout(()=>URL.revokeObjectURL(u),1000);},
     error:e=>this.error(e)});
 }
 private errorText(e:unknown){const x=e as HttpErrorResponse;return x.error?.detail||x.error?.title||x.message||'İşlem başarısız.';}
 private error(e:unknown){this.toast.add({severity:'error',summary:'İşlem başarısız',detail:this.errorText(e),life:7000});}
}

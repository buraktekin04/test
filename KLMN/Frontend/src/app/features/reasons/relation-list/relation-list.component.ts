import {Component,computed,inject,OnInit,signal} from '@angular/core';
import {HttpErrorResponse} from '@angular/common/http';
import {DatePipe} from '@angular/common';
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
import {RelationKind,RelationItem,RelationFields,RelationOptions,RelationQuery} from '../models/reason.model';
import {ReasonRelationsService} from '../services/reasons.service';
type RelationPermissionModule='ReasonForExamReportTypes'|'ReasonForExamUnusedHcAdmissionTypes'|'ReasonForExamProcedureDefs'|'ReasonForExamBranchDefs';
interface RelationConfig {label:string;shortLabel:string;slug:string;module:RelationPermissionModule;entity:string;extra:boolean;}
const configMap:Record<RelationKind,RelationConfig>={
 reportTypes:{label:'Muayene Sebebi Rapor Türleri',shortLabel:'Rapor Türü',slug:'reason-exam-report-types',module:'ReasonForExamReportTypes',entity:'ReasonForExamReportType',extra:false},
 unusedAdmissionTypes:{label:'Kullanılmayacak Hasta Grupları',shortLabel:'Hasta Grubu',slug:'reason-exam-unused-admission-types',module:'ReasonForExamUnusedHcAdmissionTypes',entity:'ReasonForExamUnusedHcAdmissionType',extra:false},
 procedures:{label:'Gereken Tetkikler',shortLabel:'Tetkik',slug:'reason-exam-procedures',module:'ReasonForExamProcedureDefs',entity:'ReasonForExamProcedureDef',extra:true},
 branches:{label:'Muayene Gereken Branşlar',shortLabel:'Branş',slug:'reason-exam-branches',module:'ReasonForExamBranchDefs',entity:'ReasonForExamBranchDef',extra:true}
};
/** Dört farklı tanım ekranını route verisi ve modüle özel permission ile yöneten ortak uygulama ekranı. */
@Component({
 selector:'app-reason-relation-list',standalone:true,
 imports:[DatePipe,RouterLink,ReactiveFormsModule,ButtonModule,CheckboxModule,
  ConfirmDialogModule,DialogModule,InputTextModule,TableModule,TagModule,ToastModule,AuditHistoryComponent],
 providers:[ConfirmationService,MessageService],
 templateUrl:'./relation-list.component.html',styleUrl:'./relation-list.component.scss'
})
export class ReasonRelationListComponent implements OnInit {
 private readonly route=inject(ActivatedRoute);
 private readonly service=inject(ReasonRelationsService);
 private readonly permissions=inject(PermissionService);
 private readonly fb=inject(FormBuilder);
 private readonly confirm=inject(ConfirmationService);
 private readonly toast=inject(MessageService);
 readonly kind=(this.route.snapshot.data['relationKind']||'reportTypes') as RelationKind;
 readonly config=configMap[this.kind];
 readonly presetReasonId=this.route.snapshot.queryParamMap.get('reasonId');
 readonly canCreate=computed(()=>this.permissions.has(PermissionCodes[this.config.module].Create));
 readonly canUpdate=computed(()=>this.permissions.has(PermissionCodes[this.config.module].Update));
 readonly canDelete=computed(()=>this.permissions.has(PermissionCodes[this.config.module].Delete));
 readonly canExport=computed(()=>this.permissions.has(PermissionCodes[this.config.module].Query));
 readonly canAudit=computed(()=>this.permissions.has(PermissionCodes.AuditLogs.View));
 readonly records=signal<RelationItem[]>([]);readonly optionsData=signal<RelationOptions>({reasons:[],references:[]});
 readonly loading=signal(false);readonly saving=signal(false);readonly exporting=signal(false);
 readonly totalCount=signal(0);readonly pageNumber=signal(1);readonly pageSize=signal(20);
 readonly totalPages=computed(()=>Math.max(1,Math.ceil(this.totalCount()/this.pageSize())));
 readonly rangeStart=computed(()=>this.totalCount()?(this.pageNumber()-1)*this.pageSize()+1:0);
 readonly rangeEnd=computed(()=>Math.min(this.totalCount(),this.pageNumber()*this.pageSize()));
 readonly editing=signal<RelationItem|null>(null);readonly auditRecord=signal<RelationItem|null>(null);
 readonly dialogVisible=signal(false);readonly filtersVisible=signal(false);readonly attemptedSave=signal(false);
 readonly formError=signal<string|null>(null);
 readonly filterForm=this.fb.nonNullable.group({search:'',includeInactive:false});
 readonly columnFilterForm=this.fb.group({
   isActive:this.fb.control<boolean|null>(null),
   reasonForExaminationDefId:this.fb.control<string|null>(this.presetReasonId)
 });
 readonly itemForm=this.fb.nonNullable.group({
   reasonForExaminationDefId:['',Validators.required],
   referenceId:['',Validators.required],
   startingAge:[0,[Validators.min(0),Validators.max(130)]],
   endingAge:[130,[Validators.min(0),Validators.max(130)]],
   gender:[0,[Validators.min(0),Validators.max(20)]]
 });
 private requestId=0;
 ngOnInit():void{this.loadOptions(this.presetReasonId);this.load();}
 private query():RelationQuery{
 const f=this.filterForm.getRawValue(),c=this.columnFilterForm.getRawValue();
 return {search:f.search,includeInactive:f.includeInactive||c.isActive===false,isActive:c.isActive,
 reasonForExaminationDefId:c.reasonForExaminationDefId,pageNumber:this.pageNumber(),pageSize:this.pageSize()};
 }
 /** Seçili geçmiş pasif FK değerlerini de seçeneklerde gösterir. */
 private loadOptions(reasonId?:string|null,refId?:string|null):void{
 this.service.options(this.config.slug,reasonId,refId)
 .subscribe({next:x=>this.optionsData.set(x),error:e=>this.notice(e)});
 }
 load():void {
 const n=++this.requestId;this.loading.set(true);
 this.service.list(this.config.slug,this.query()).pipe(finalize(()=>{if(n===this.requestId)this.loading.set(false);})).subscribe({
 next:r=>{if(n!==this.requestId)return;this.records.set(r.items);this.totalCount.set(r.totalCount);},
 error:e=>{if(n!==this.requestId)return;this.records.set([]);this.totalCount.set(0);this.notice(e);}
 });
 }
 search():void{this.pageNumber.set(1);this.load();}
 clear():void {this.filterForm.reset({search:'',includeInactive:false});
 this.columnFilterForm.reset({isActive:null,reasonForExaminationDefId:this.presetReasonId});this.search();}
 page(p:number):void{if(p<1||p>this.totalPages()||this.loading())return;this.pageNumber.set(p);this.load();}
 size(event:Event):void{const s=Number((event.target as HTMLSelectElement).value);
 if(![10,20,50,100].includes(s))return;this.pageSize.set(s);this.search();}
 openCreate(reasonId?:string):void {
 if(!this.canCreate())return;
 this.editing.set(null);this.attemptedSave.set(false);this.formError.set(null);
 const id=reasonId||this.presetReasonId||this.columnFilterForm.value.reasonForExaminationDefId||'';
 this.itemForm.reset({reasonForExaminationDefId:id,referenceId:'',startingAge:0,endingAge:130,gender:0});
 this.loadOptions(id);this.dialogVisible.set(true);
 }
 openEdit(item:RelationItem):void {
 this.service.getById(this.config.slug,item.id).subscribe({next:d=>{
 this.editing.set(d);this.formError.set(null);this.attemptedSave.set(false);
 this.itemForm.reset({reasonForExaminationDefId:d.reasonForExaminationDefId,referenceId:d.referenceId,
 startingAge:d.startingAge??0,endingAge:d.endingAge??130,gender:d.gender??0});
 this.loadOptions(d.reasonForExaminationDefId,d.referenceId);this.dialogVisible.set(true);
 },error:e=>this.notice(e)});
 }
 save():void{
 if(this.saving()||(this.editing()?!this.canUpdate():!this.canCreate()))return;
 this.attemptedSave.set(true);this.formError.set(null);
 const v=this.itemForm.getRawValue();
 if(this.itemForm.invalid||(this.config.extra&&v.startingAge>v.endingAge)){
 this.itemForm.markAllAsTouched();this.formError.set('Zorunlu alanları ve yaş aralığını kontrol ediniz.');return;}
 const fields:RelationFields={reasonForExaminationDefId:v.reasonForExaminationDefId,
 referenceId:v.referenceId,...(this.config.extra?{startingAge:v.startingAge,endingAge:v.endingAge,gender:v.gender}:{})};
 const d=this.editing();const request=d?this.service.update(this.config.slug,{...fields,id:d.id,version:d.version})
 :this.service.create(this.config.slug,fields);
 this.saving.set(true);request.pipe(finalize(()=>this.saving.set(false))).subscribe({
 next:()=>{this.dialogVisible.set(false);this.load();this.toast.add({severity:'success',summary:'Kaydedildi',detail:this.config.label+' kaydedildi.'});},
 error:e=>{this.formError.set(this.errorText(e));this.notice(e);}
 });
 }
 changeStatus(item:RelationItem):void{
 if(!this.canUpdate())return;
 this.confirm.confirm({header:'Durum Değiştir',message:'İlgili eşleştirmenin aktif/pasif durumu değiştirilsin mi?',
 acceptLabel:'Onayla',rejectLabel:'Vazgeç',accept:()=>{
 this.service.changeStatus(this.config.slug,item.id,item.version,!item.isActive)
 .subscribe({next:()=>this.load(),error:e=>this.notice(e)});}});
 }
 delete(item:RelationItem):void{
 if(!this.canDelete())return;
 this.confirm.confirm({header:'Silme Onayı',message:'Bu eşleştirme silinsin mi?',
 acceptLabel:'Sil',rejectLabel:'Vazgeç',accept:()=>{
 this.service.delete(this.config.slug,item.id,item.version).subscribe({next:()=>this.load(),error:e=>this.notice(e)});}});
 }
 exportExcel():void{
 if(!this.canExport())return;this.exporting.set(true);
 this.service.export(this.config.slug,this.query()).pipe(finalize(()=>this.exporting.set(false)))
 .subscribe({next:blob=>{const u=URL.createObjectURL(blob),a=document.createElement('a');
 a.href=u;a.download=this.config.slug+'.xlsx';a.click();setTimeout(()=>URL.revokeObjectURL(u),1000);},
 error:e=>this.notice(e)});
 }
 private errorText(error:unknown):string{const e=error as HttpErrorResponse;
 return e.error?.detail||e.error?.title||e.message||'İşlem gerçekleştirilemedi.';}
 private notice(error:unknown):void{this.toast.add({severity:'error',summary:'İşlem başarısız',detail:this.errorText(error),life:6500});}
}

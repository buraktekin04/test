import {Component,computed,inject,OnInit,signal} from '@angular/core';
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
import {HcDecisionFields,HcDecisionItem,HcDecisionQuery} from '../models/hc-decision.model';
import {HcDecisionsService} from '../services/hc-decisions.service';
type Field='code'|'name'|'validTo';
const relations=[{"kind":"unusedReasons","label":"Kullanılmayacak Muayene Sebepleri","slug":"hc-unused-reasons","module":"HcUnusedReasons"},{"kind":"unusedAdmissionTypes","label":"Kullanılmayacak Başvuru Grupları","slug":"hc-unused-admission-types","module":"HcUnusedAdmissionTypes"},{"kind":"unusedDecisions","label":"Birlikte Verilmeyecek Kararlar","slug":"hc-unused-decisions","module":"HcUnusedDecisions"},{"kind":"connectedDecisions","label":"Bağlı Kararlar","slug":"hc-decision-connections","module":"HcDecisionConnections"}] as const;
/** Kurul kararlarının bağımsız ekranı; bağlı dört tanımı listede gösterir. */
@Component({
 selector:'app-hc-decision-list',standalone:true,
 imports:[ReactiveFormsModule,ButtonModule,CheckboxModule,ConfirmDialogModule,
 DialogModule,InputTextModule,TableModule,TagModule,ToastModule,AuditHistoryComponent],
 providers:[ConfirmationService,MessageService],
 templateUrl:'./hc-decision-list.component.html',styleUrl:'./hc-decision-list.component.scss'
})
export class HcDecisionListComponent implements OnInit {
 private service=inject(HcDecisionsService);
 private permissions=inject(PermissionService);
 private fb=inject(FormBuilder);
 private confirm=inject(ConfirmationService);
 private toast=inject(MessageService);
 private router=inject(Router);
 readonly relationColumns=relations;
 readonly canCreate=computed(()=>this.permissions.has(PermissionCodes.HcDecisions.Create));
 readonly canUpdate=computed(()=>this.permissions.has(PermissionCodes.HcDecisions.Update));
 readonly canDelete=computed(()=>this.permissions.has(PermissionCodes.HcDecisions.Delete));
 readonly canExport=computed(()=>this.permissions.has(PermissionCodes.HcDecisions.Query));
 readonly canAudit=computed(()=>this.permissions.has(PermissionCodes.AuditLogs.View));
 readonly records=signal<HcDecisionItem[]>([]);
 readonly totalCount=signal(0);readonly pageNumber=signal(1);readonly pageSize=signal(20);
 readonly loading=signal(false);readonly saving=signal(false);readonly exporting=signal(false);
 readonly dialogVisible=signal(false);readonly editing=signal<HcDecisionItem|null>(null);
 readonly auditRecord=signal<HcDecisionItem|null>(null);readonly filtersVisible=signal(false);
 readonly attemptedSave=signal(false);readonly formError=signal<string|null>(null);
 readonly totalPages=computed(()=>Math.max(1,Math.ceil(this.totalCount()/this.pageSize())));
 readonly rangeStart=computed(()=>this.totalCount()?(this.pageNumber()-1)*this.pageSize()+1:0);
 readonly rangeEnd=computed(()=>Math.min(this.totalCount(),this.pageNumber()*this.pageSize()));
 readonly filterForm=this.fb.nonNullable.group({search:'',includeInactive:false});
 readonly columnFilterForm=this.fb.group({isActive:this.fb.control<boolean|null>(null)});
 readonly itemForm=this.fb.nonNullable.group({
  code:[0,[Validators.required,Validators.min(0)]],name:['',[Validators.required,Validators.maxLength(500)]],
  timeInformationRequired:false,additionalDescriptionRequired:false,negativeDecision:false,approvalRequired:false,pertemApprovalRequired:false,diagnosisRequired:false,validFrom:'',validTo:''
 });
 private requestId=0;
 ngOnInit():void{this.load();}
 canOpenRelation(module:typeof relations[number]['module']):boolean{return this.permissions.hasModule(module);}
 private query():HcDecisionQuery{const f=this.filterForm.getRawValue(),c=this.columnFilterForm.getRawValue();
 return {search:f.search,includeInactive:f.includeInactive||c.isActive===false,isActive:c.isActive,pageNumber:this.pageNumber(),pageSize:this.pageSize()};}
 /** Filtre/sayfalama API'de; istemci sıralaması yalnız yüklü sayfada uygulanır. */
 load():void{const n=++this.requestId;this.loading.set(true);
 this.service.list(this.query()).pipe(finalize(()=>{if(n===this.requestId)this.loading.set(false);})).subscribe({
 next:r=>{if(n!==this.requestId)return;this.records.set(r.items);this.totalCount.set(r.totalCount);},
 error:e=>{if(n!==this.requestId)return;this.records.set([]);this.totalCount.set(0);this.notice(e);}});
 }
 search():void{this.pageNumber.set(1);this.load();}
 clear():void{this.filterForm.reset({search:'',includeInactive:false});this.columnFilterForm.reset({isActive:null});this.search();}
 page(p:number):void{if(p<1||p>this.totalPages()||this.loading())return;this.pageNumber.set(p);this.load();}
 size(e:Event):void{const s=Number((e.target as HTMLSelectElement).value);if(![10,20,50,100].includes(s))return;this.pageSize.set(s);this.search();}
 openCreate():void{this.editing.set(null);this.attemptedSave.set(false);this.formError.set(null);
 this.itemForm.reset({code:0,name:'',timeInformationRequired:false,additionalDescriptionRequired:false,negativeDecision:false,approvalRequired:false,pertemApprovalRequired:false,diagnosisRequired:false,validFrom:'',validTo:''});this.dialogVisible.set(true);}
 openEdit(item:HcDecisionItem):void{
 this.service.getById(item.id).subscribe({next:d=>{this.editing.set(d);this.formError.set(null);this.attemptedSave.set(false);
 this.itemForm.reset({code:d.code,name:d.name,timeInformationRequired:d.timeInformationRequired,additionalDescriptionRequired:d.additionalDescriptionRequired,negativeDecision:d.negativeDecision,approvalRequired:d.approvalRequired,pertemApprovalRequired:d.pertemApprovalRequired,diagnosisRequired:d.diagnosisRequired,
 validFrom:ValidityPeriodUtils.toLocalInput(d.validFrom),validTo:ValidityPeriodUtils.toLocalInput(d.validTo)});
 this.dialogVisible.set(true);},error:e=>this.notice(e)});}
 fieldError(field:Field):string|null{const c=this.itemForm.controls[field];
 if(!this.attemptedSave()&&!c.touched)return null;
 if(c.hasError('required'))return 'Bu alan zorunludur.';
 if(c.hasError('maxlength'))return 'Karakter sınırı aşıldı.';
 if(c.hasError('min'))return 'Kod negatif olamaz.';
 if(field==='validTo'&&!ValidityPeriodUtils.hasValidRange(this.itemForm.value.validFrom??'',this.itemForm.value.validTo??''))return 'Bitiş tarihi başlangıçtan sonra olmalıdır.';
 return null;}
 save():void{
 if(this.saving()||(this.editing()?!this.canUpdate():!this.canCreate()))return;
 this.attemptedSave.set(true);this.formError.set(null);
 const v=this.itemForm.getRawValue();
 if(this.itemForm.invalid||!Number.isInteger(v.code)||!ValidityPeriodUtils.hasValidRange(v.validFrom,v.validTo)){
 this.itemForm.markAllAsTouched();this.formError.set('Kod, karar adı ve tarih aralığını kontrol ediniz.');return;}
 const fields:HcDecisionFields={code:v.code,name:v.name.trim(),timeInformationRequired:v.timeInformationRequired,additionalDescriptionRequired:v.additionalDescriptionRequired,negativeDecision:v.negativeDecision,approvalRequired:v.approvalRequired,pertemApprovalRequired:v.pertemApprovalRequired,diagnosisRequired:v.diagnosisRequired,
 ...ValidityPeriodUtils.fromLocalInputs(v.validFrom,v.validTo)};
 const d=this.editing();const req=d?this.service.update({...fields,id:d.id,version:d.version}):this.service.create(fields);
 this.saving.set(true);req.pipe(finalize(()=>this.saving.set(false))).subscribe({
 next:()=>{this.dialogVisible.set(false);this.toast.add({severity:'success',summary:'Kaydedildi',detail:'Kurul kararı kaydedildi.'});this.load();},
 error:e=>{this.formError.set(this.errorText(e));this.notice(e);}});
 }
 changeStatus(item:HcDecisionItem):void{if(!this.canUpdate())return;
 this.confirm.confirm({header:'Durum Değiştir',message:'Kurul kararının durumu değiştirilsin mi?',acceptLabel:'Onayla',rejectLabel:'Vazgeç',
 accept:()=>this.service.changeStatus(item.id,item.version,!item.isActive).subscribe({next:()=>this.load(),error:e=>this.notice(e)})});}
 delete(item:HcDecisionItem):void{if(!this.canDelete())return;
 this.confirm.confirm({header:'Silme Onayı',message:'Karar silinsin mi? Bağlı kayıtlar varsa engellenecektir.',acceptLabel:'Sil',rejectLabel:'Vazgeç',
 accept:()=>this.service.delete(item.id,item.version).subscribe({next:()=>this.load(),error:e=>this.notice(e)})});}
 openRelation(item:HcDecisionItem,slug:string):void{void this.router.navigate(['/app',slug],{queryParams:{decisionId:item.id}});}
 exportExcel():void{if(!this.canExport())return;this.exporting.set(true);
 this.service.export(this.query()).pipe(finalize(()=>this.exporting.set(false))).subscribe({
 next:blob=>{const u=URL.createObjectURL(blob),a=document.createElement('a');a.href=u;a.download='Kurul_Karar_Tanimlari.xlsx';a.click();setTimeout(()=>URL.revokeObjectURL(u),1000);},
 error:e=>this.notice(e)});}
 private errorText(error:unknown):string{const e=error as HttpErrorResponse;return e.error?.detail||e.error?.title||e.message||'İşlem gerçekleştirilemedi.';}
 private notice(error:unknown):void{this.toast.add({severity:'error',summary:'İşlem başarısız',detail:this.errorText(error),life:6500});}
}

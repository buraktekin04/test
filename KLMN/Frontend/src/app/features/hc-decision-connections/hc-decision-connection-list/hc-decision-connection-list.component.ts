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
import {SelectModule} from 'primeng/select';
import {TableModule} from 'primeng/table';
import {TagModule} from 'primeng/tag';
import {ToastModule} from 'primeng/toast';
import {AuditHistoryComponent} from '../../../shared/audit-history/audit-history.component';
import {PermissionCodes} from '../../../core/permissions/constants/permission-codes';
import {PermissionService} from '../../../core/permissions/services/permission.service';
import {HcDecisionRelationItem,HcDecisionRelationFields,HcDecisionRelationOptions,HcDecisionRelationQuery} from '../models/hc-decision-connection.model';
import {HcDecisionConnectionsService} from '../services/hc-decision-connection.service';
/** Bağlı Kararlar: kendi HTML, stil, model, servis ve permission'a sahip bağımsız ekran. */
@Component({
 selector:'app-hc-decision-connection-list',standalone:true,
 imports:[RouterLink,ReactiveFormsModule,ButtonModule,CheckboxModule,ConfirmDialogModule,DialogModule,InputTextModule,SelectModule,TableModule,TagModule,ToastModule,AuditHistoryComponent],
 providers:[ConfirmationService,MessageService],templateUrl:'./hc-decision-connection-list.component.html',styleUrl:'./hc-decision-connection-list.component.scss'
})
export class HcDecisionConnectionListComponent implements OnInit {
 private route=inject(ActivatedRoute);
 private service=inject(HcDecisionConnectionsService);
 private permissions=inject(PermissionService);
 private fb=inject(FormBuilder);
 private confirm=inject(ConfirmationService);
 private toast=inject(MessageService);
 readonly title='Bağlı Kararlar';
 readonly shortLabel='Bağlı Kurul Kararı';
 readonly canCreate=computed(()=>this.permissions.has(PermissionCodes.HcDecisionConnections.Create));
 readonly canUpdate=computed(()=>this.permissions.has(PermissionCodes.HcDecisionConnections.Update));
 readonly canDelete=computed(()=>this.permissions.has(PermissionCodes.HcDecisionConnections.Delete));
 readonly canExport=computed(()=>this.permissions.has(PermissionCodes.HcDecisionConnections.Query));
 readonly canAudit=computed(()=>this.permissions.has(PermissionCodes.AuditLogs.View));
 readonly presetDecisionId=this.route.snapshot.queryParamMap.get('decisionId');
 readonly records=signal<HcDecisionRelationItem[]>([]);
 readonly optionsData=signal<HcDecisionRelationOptions>({decisions:[],references:[]});
 readonly totalCount=signal(0);readonly pageNumber=signal(1);readonly pageSize=signal(20);
 readonly totalPages=computed(()=>Math.max(1,Math.ceil(this.totalCount()/this.pageSize())));
 readonly rangeStart=computed(()=>this.totalCount()?(this.pageNumber()-1)*this.pageSize()+1:0);
 readonly rangeEnd=computed(()=>Math.min(this.totalCount(),this.pageNumber()*this.pageSize()));
 readonly loading=signal(false);readonly saving=signal(false);readonly exporting=signal(false);
 readonly editing=signal<HcDecisionRelationItem|null>(null);
 readonly auditRecord=signal<HcDecisionRelationItem|null>(null);
 readonly dialogVisible=signal(false);readonly filtersVisible=signal(false);
 readonly formError=signal<string|null>(null);readonly attemptedSave=signal(false);
 readonly filterForm=this.fb.nonNullable.group({search:'',includeInactive:false});
 readonly columnFilterForm=this.fb.group({isActive:this.fb.control<boolean|null>(null),hcDecisionDefId:this.fb.control<string|null>(this.presetDecisionId)});
 readonly itemForm=this.fb.nonNullable.group({hcDecisionDefId:['',Validators.required],referenceId:['',Validators.required]});
 private requestId=0;
 ngOnInit():void{this.loadOptions(this.presetDecisionId);this.load();}
 private query():HcDecisionRelationQuery{const f=this.filterForm.getRawValue(),c=this.columnFilterForm.getRawValue();
 return {search:f.search,includeInactive:f.includeInactive||c.isActive===false,isActive:c.isActive,hcDecisionDefId:c.hcDecisionDefId,pageNumber:this.pageNumber(),pageSize:this.pageSize()};}
 /** Seçilen geçmiş pasif referansları görüntülenebilir; yeni seçimler backend'de aktiftir. */
 private loadOptions(id?:string|null,referenceId?:string|null):void{
 this.service.options(id,referenceId).subscribe({next:x=>this.optionsData.set(x),error:e=>this.notice(e)});}
 load():void{const n=++this.requestId;this.loading.set(true);
 this.service.list(this.query()).pipe(finalize(()=>{if(n===this.requestId)this.loading.set(false);})).subscribe({
 next:r=>{if(n!==this.requestId)return;this.records.set(r.items);this.totalCount.set(r.totalCount);},
 error:e=>{if(n!==this.requestId)return;this.records.set([]);this.totalCount.set(0);this.notice(e);}});}
 search():void{this.pageNumber.set(1);this.load();}
 clear():void{this.filterForm.reset({search:'',includeInactive:false});this.columnFilterForm.reset({isActive:null,hcDecisionDefId:this.presetDecisionId});this.search();}
 page(p:number):void{if(p<1||p>this.totalPages()||this.loading())return;this.pageNumber.set(p);this.load();}
 size(e:Event):void{const s=Number((e.target as HTMLSelectElement).value);if(![10,20,50,100].includes(s))return;this.pageSize.set(s);this.search();}
 openCreate():void{
 if(!this.canCreate())return;
 const id=this.presetDecisionId||this.columnFilterForm.value.hcDecisionDefId||'';
 this.editing.set(null);this.formError.set(null);this.attemptedSave.set(false);
 this.itemForm.reset({hcDecisionDefId:id,referenceId:''});this.loadOptions(id);this.dialogVisible.set(true);}
 openEdit(item:HcDecisionRelationItem):void{
 this.service.getById(item.id).subscribe({next:d=>{this.editing.set(d);this.formError.set(null);this.attemptedSave.set(false);
 this.itemForm.reset({hcDecisionDefId:d.hcDecisionDefId,referenceId:d.referenceId});
 this.loadOptions(d.hcDecisionDefId,d.referenceId);this.dialogVisible.set(true);},error:e=>this.notice(e)});}
 save():void{
 if(this.saving()||(this.editing()?!this.canUpdate():!this.canCreate()))return;
 this.attemptedSave.set(true);this.formError.set(null);
 if(this.itemForm.invalid){this.itemForm.markAllAsTouched();this.formError.set('Kurul kararı ve referans kaydı seçiniz.');return;}
 const v=this.itemForm.getRawValue();
 const fields:HcDecisionRelationFields={hcDecisionDefId:v.hcDecisionDefId,referenceId:v.referenceId};
 const d=this.editing();const req=d?this.service.update({...fields,id:d.id,version:d.version}):this.service.create(fields);
 this.saving.set(true);req.pipe(finalize(()=>this.saving.set(false))).subscribe({
 next:()=>{this.dialogVisible.set(false);this.load();this.toast.add({severity:'success',summary:'Kaydedildi',detail:this.title+' kaydedildi.'});},
 error:e=>{this.formError.set(this.errorText(e));this.notice(e);}});
 }
 changeStatus(item:HcDecisionRelationItem):void{if(!this.canUpdate())return;
 this.confirm.confirm({header:'Durum Değiştir',message:'Eşleştirme durumu değiştirilsin mi?',acceptLabel:'Onayla',rejectLabel:'Vazgeç',
 accept:()=>this.service.changeStatus(item.id,item.version,!item.isActive).subscribe({next:()=>this.load(),error:e=>this.notice(e)})});}
 delete(item:HcDecisionRelationItem):void{if(!this.canDelete())return;
 this.confirm.confirm({header:'Silme Onayı',message:'Eşleştirme silinsin mi?',acceptLabel:'Sil',rejectLabel:'Vazgeç',
 accept:()=>this.service.delete(item.id,item.version).subscribe({next:()=>this.load(),error:e=>this.notice(e)})});}
 exportExcel():void{if(!this.canExport())return;this.exporting.set(true);
 this.service.export(this.query()).pipe(finalize(()=>this.exporting.set(false))).subscribe({
 next:blob=>{const u=URL.createObjectURL(blob),a=document.createElement('a');a.href=u;a.download='hc-decision-connections.xlsx';a.click();setTimeout(()=>URL.revokeObjectURL(u),1000);},
 error:e=>this.notice(e)});}
 private errorText(error:unknown):string{const e=error as HttpErrorResponse;return e.error?.detail||e.error?.title||e.message||'İşlem gerçekleştirilemedi.';}
 private notice(error:unknown):void{this.toast.add({severity:'error',summary:'İşlem başarısız',detail:this.errorText(error),life:6500});}
}

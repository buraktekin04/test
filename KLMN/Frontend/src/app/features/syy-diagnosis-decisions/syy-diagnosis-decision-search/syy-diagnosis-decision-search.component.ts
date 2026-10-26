import {Component,computed,inject,OnInit,signal} from '@angular/core';
import {HttpErrorResponse} from '@angular/common/http';
import {FormBuilder,ReactiveFormsModule} from '@angular/forms';
import {RouterLink} from '@angular/router';
import {finalize} from 'rxjs';
import {ButtonModule} from 'primeng/button';
import {TableModule} from 'primeng/table';
import {TagModule} from 'primeng/tag';
import {ToastModule} from 'primeng/toast';
import {MessageService} from 'primeng/api';
import {PermissionCodes} from '../../../core/permissions/constants/permission-codes';
import {PermissionService} from '../../../core/permissions/services/permission.service';
import {SyyDiagnosisDecisionService} from '../services/syy-diagnosis-decision.service';
import {SyyDiagnosisDecisionItem,SyyDiagnosisDecisionOptions,SyyDiagnosisDecisionQuery} from '../models/syy-diagnosis-decision.model';
/** Yalnız okuma amaçlı SYY Tanı Karar İstatistiği Sorgulama ekranı.
 * SYY Tanı Kodu, Başvuru Grubu ve Askerî Sınıf filtreleri ayrı dropdown'lardır.
 */
@Component({
 selector:'app-syy-diagnosis-decision-search',standalone:true,
 imports:[RouterLink,ReactiveFormsModule,ButtonModule,TableModule,TagModule,ToastModule],
 providers:[MessageService],
 templateUrl:'./syy-diagnosis-decision-search.component.html',
 styleUrl:'./syy-diagnosis-decision-search.component.scss'
})
export class SyyDiagnosisDecisionSearchComponent implements OnInit {
 private readonly api=inject(SyyDiagnosisDecisionService);
 private readonly permissions=inject(PermissionService);
 private readonly fb=inject(FormBuilder);
 private readonly message=inject(MessageService);
 readonly canExport=computed(()=>this.permissions.has(PermissionCodes.SYYDiagnosisDecisions.Query));
 readonly canDefinitions=computed(()=>this.permissions.hasModule('SYYDiagnosisDecisions'));
 readonly optionsData=signal<SyyDiagnosisDecisionOptions>({syyDiagnoses:[],decisions:[],admissionTypes:[],militaryClasses:[]});
 readonly optionsLoading=signal(false);
 readonly rows=signal<SyyDiagnosisDecisionItem[]>([]);
 readonly page=signal(1);readonly pageSize=signal(20);readonly total=signal(0);
 readonly maxPage=computed(()=>Math.max(1,Math.ceil(this.total()/this.pageSize())));
 readonly visiblePageAmount=computed(()=>this.rows().reduce((sum,r)=>sum+r.amount,0));
 readonly loading=signal(false);readonly exporting=signal(false);readonly ranQuery=signal(false);
 readonly filters=this.fb.nonNullable.group({
   syyDiagnosisDefId:'',hcAdmissionTypeDefId:'',militaryClassDefId:'',hcDecisionDefId:'',
   includeInactive:false
 });
 private requestId=0;
 ngOnInit():void{
  this.optionsLoading.set(true);
  this.api.options(undefined,true).pipe(finalize(()=>this.optionsLoading.set(false))).subscribe({
    next:options=>this.optionsData.set(options),error:e=>this.notice(e)
  });
  this.load();
 }
 private query():SyyDiagnosisDecisionQuery {
  const f=this.filters.getRawValue();
  return {search:'',includeInactive:f.includeInactive,isActive:null,pageNumber:this.page(),pageSize:this.pageSize(),
   syyDiagnosisDefId:f.syyDiagnosisDefId||null,hcAdmissionTypeDefId:f.hcAdmissionTypeDefId||null,
   militaryClassDefId:f.militaryClassDefId||null,hcDecisionDefId:f.hcDecisionDefId||null};
 }
 load():void{
  const id=++this.requestId;this.loading.set(true);this.ranQuery.set(true);
  this.api.search(this.query()).pipe(finalize(()=>{if(id===this.requestId)this.loading.set(false);})).subscribe({
    next:r=>{if(id!==this.requestId)return;this.rows.set(r.items);this.total.set(r.totalCount);},
    error:e=>{if(id!==this.requestId)return;this.rows.set([]);this.total.set(0);this.notice(e);}
  });
 }
 search():void{this.page.set(1);this.load();}
 clear():void{
  this.filters.reset({syyDiagnosisDefId:'',hcAdmissionTypeDefId:'',militaryClassDefId:'',hcDecisionDefId:'',includeInactive:false});
  this.search();
 }
 toPage(p:number):void{if(p<1||p>this.maxPage()||this.loading())return;this.page.set(p);this.load();}
 changeSize(e:Event):void{this.pageSize.set(Number((e.target as HTMLSelectElement).value));this.search();}
 exportExcel():void{
  if(!this.canExport())return;this.exporting.set(true);
  this.api.export(this.query()).pipe(finalize(()=>this.exporting.set(false))).subscribe({
   next:blob=>{const url=URL.createObjectURL(blob);const a=document.createElement('a');
    a.href=url;a.download='syy-tani-karar-istatistigi-sorgu.xlsx';a.click();
    setTimeout(()=>URL.revokeObjectURL(url),1000);},error:e=>this.notice(e)});
 }
 private notice(error:unknown):void{
  const e=error as HttpErrorResponse;
  this.message.add({severity:'error',summary:'Sorgu başarısız',detail:e.error?.detail||e.error?.title||e.message||'İşlem gerçekleştirilemedi.',life:7000});
 }
}

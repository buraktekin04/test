import {DatePipe} from '@angular/common';
import {HttpErrorResponse} from '@angular/common/http';
import {AfterViewInit,Component,computed,inject,OnInit,signal} from '@angular/core';
import {FormBuilder,ReactiveFormsModule} from '@angular/forms';
import {ActivatedRoute,Router} from '@angular/router';
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
import {PermissionCodes} from '../../../core/permissions/constants/permission-codes';
import {PermissionService} from '../../../core/permissions/services/permission.service';
import {AuditHistoryComponent} from '../../../shared/audit-history/audit-history.component';
import {DispatchItem,DispatchOption,DispatchStatusOption,DispatchAdvancedFilters} from '../models/dispatch.model';
import {DispatchService} from '../services/dispatch.service';

/** Sevk listesi ve sunucu üzerinde sorgulama. Sevk giriş formunu içermez. */
@Component({
 selector:'app-dispatch-list',standalone:true,
 imports:[DatePipe,ReactiveFormsModule,ButtonModule,CheckboxModule,ConfirmDialogModule,
  DialogModule,InputTextModule,SelectModule,TableModule,TagModule,ToastModule,AuditHistoryComponent],
 providers:[ConfirmationService,MessageService],
 templateUrl:'./dispatch-list.component.html',
 styleUrl:'./dispatch-list.component.scss'
})
export class DispatchListComponent implements OnInit,AfterViewInit {
 private readonly api=inject(DispatchService);
 private readonly fb=inject(FormBuilder);
 private readonly permissions=inject(PermissionService);
 private readonly confirm=inject(ConfirmationService);
 private readonly message=inject(MessageService);
 private readonly router=inject(Router);
 private readonly route=inject(ActivatedRoute);

 readonly canView=computed(()=>this.permissions.hasAny(PermissionCodes.Dispatches.View,PermissionCodes.Dispatches.Query));
 readonly canCreate=computed(()=>this.permissions.has(PermissionCodes.Dispatches.Create));
 readonly canUpdate=computed(()=>this.permissions.has(PermissionCodes.Dispatches.Update));
 readonly canAudit=computed(()=>this.permissions.has(PermissionCodes.AuditLogs.View));

 readonly records=signal<DispatchItem[]>([]);
 readonly total=signal(0);
 readonly page=signal(1);
 readonly pageSize=signal(20);
 readonly maxPage=computed(()=>Math.max(1,Math.ceil(this.total()/this.pageSize())));
 readonly loading=signal(false);
 readonly hasSearched=signal(false);
 readonly auditRecord=signal<DispatchItem|null>(null);
 private lastRequest=0;

 readonly statuses=signal<DispatchStatusOption[]>([]);
 readonly statusItems=computed(()=>[
  {label:'Tüm durumlar',value:null as number|null},
  ...this.statuses().map(x=>({label:x.name,value:x.code as number|null}))
 ]);
 readonly scopeItems=[
  {label:'Tüm kapsamlar',value:null as number|null},
  {label:'Kapsam belirtilmemiş',value:1},
  {label:'Şehirdeki sağlık tesisleri',value:2},
  {label:'Belirli sağlık tesisi',value:3}
 ];
 readonly optionMap=signal<Record<'units'|'offices'|'cities'|'hospitals',DispatchOption[]>>({
  units:[],offices:[],cities:[],hospitals:[]
 });
 readonly senderItems=computed(()=>[
  ...this.optionMap().units.map(x=>({label:'Birlik · '+x.label,value:'unit:'+x.id})),
  ...this.optionMap().offices.map(x=>({label:'Askerlik Şubesi · '+x.label,value:'office:'+x.id}))
 ]);
 private optionRequest:Partial<Record<'units'|'offices'|'cities'|'hospitals',number>>={};
 readonly searchForm=this.fb.group({
  search:this.fb.nonNullable.control(''),
  includeCancelled:this.fb.nonNullable.control(false),
  currentState:this.fb.control<number|null>(null),
  dateFrom:this.fb.nonNullable.control(''),
  dateTo:this.fb.nonNullable.control(''),
  dispatchNo:this.fb.control<number|null>(null),
  purchaseId:this.fb.control<number|null>(null),
  tcKimlikNo:this.fb.nonNullable.control(''),
  yabanciKimlikNo:this.fb.nonNullable.control(''),
  name:this.fb.nonNullable.control(''),
  surname:this.fb.nonNullable.control(''),
  senderAuthority:this.fb.control<string|null>(null),
  includeRelatedOrganizations:this.fb.nonNullable.control(false),
  dispatchScope:this.fb.control<number|null>(null),
  dispatchScopeCityId:this.fb.control<string|null>(null),
  dispatchScopeHospitalId:this.fb.control<string|null>(null)
 });
 readonly filterError=signal<string|null>(null);
 /** Sayfalama yalnız son 'Sorgula' ile uygulanmış filtrelerle devam eder. */
 private appliedForm:ReturnType<typeof this.searchForm.getRawValue>|null=null;
 private statusesLoaded=false;
 private statusesLoading=false;

 ngOnInit(){
  this.searchForm.controls.senderAuthority.valueChanges.subscribe(value=>{
   if(!value?.startsWith('unit:'))this.searchForm.controls.includeRelatedOrganizations.setValue(false);
  });
  this.searchForm.controls.dispatchScope.valueChanges.subscribe(value=>{
   if(value!==2)this.searchForm.controls.dispatchScopeCityId.setValue(null);
   if(value!==3)this.searchForm.controls.dispatchScopeHospitalId.setValue(null);
  });
 }
 /** Durumlar ilk açılışta değil filtre listesi açılınca veya sorgu istenince yüklenir. */
 loadStatuses(){
  if(!this.canView()||this.statusesLoaded||this.statusesLoading)return;
  this.statusesLoading=true;
  this.api.statuses().subscribe({
   next:items=>{this.statuses.set(items);this.statusesLoaded=true;this.statusesLoading=false;},
   error:e=>{this.statusesLoading=false;this.error(e);}
  });
 }
 /** PrimeNG açılır listeleri backend üzerinden gerektiğinde arar. */
 loadOptions(type:'units'|'offices'|'cities'|'hospitals',search=''){
  const request=(this.optionRequest[type]??0)+1;
  this.optionRequest[type]=request;
  const sender=this.searchForm.controls.senderAuthority.value;
  const selectedId=type==='units'&&sender?.startsWith('unit:')?sender.slice(5):
   type==='offices'&&sender?.startsWith('office:')?sender.slice(7):
   type==='cities'?this.searchForm.controls.dispatchScopeCityId.value??undefined:
   type==='hospitals'?this.searchForm.controls.dispatchScopeHospitalId.value??undefined:undefined;
  this.api.options(type,search,selectedId).subscribe({
   next:values=>{if(this.optionRequest[type]===request)
    this.optionMap.update(map=>({...map,[type]:values}));},
   error:e=>{if(this.optionRequest[type]===request)this.error(e);}
  });
 }
 filterSenderOptions(search:string){
  this.loadOptions('units',search??'');
  this.loadOptions('offices',search??'');
 }
 /** Seçili gönderen birlikse alt organizasyon filtresi kullanılabilir. */
 canIncludeRelated():boolean{
  return this.searchForm.controls.senderAuthority.value?.startsWith('unit:')??false;
 }


 /** PrimeNG toast view hazır olduktan sonra kayıt sonucu gösterilir. */
 ngAfterViewInit(){
  const saved=this.route.snapshot.queryParamMap.get('saved');
  if(!saved||!/^\d+$/.test(saved))return;
  const hasFailures=this.route.snapshot.queryParamMap.get('attachmentsFailed');
  this.message.add({
   severity:hasFailures&&Number(hasFailures)>0?'warn':'success',
   summary:hasFailures&&Number(hasFailures)>0?'Sevk kaydedildi; ekleri kontrol edin':'Sevk başarıyla kaydedildi',
   detail:saved+' numaralı sevkin kayıt işlemi tamamlandı.'+
    (hasFailures&&Number(hasFailures)>0?' Bazı dosyalar yüklenememiş olabilir.':''),
   life:7500
  });
 }

 /** Yalnız backend üzerinde filtrelenmiş sayfayı çağırır; sayfalar arası sonuç kaybetmez. */
 load(){
  if(!this.canView()||!this.appliedForm)return;
  const request=++this.lastRequest;
  this.loading.set(true);
  const form=this.appliedForm;
  const authority=form.senderAuthority;
  const filters:DispatchAdvancedFilters={
   dateFrom:form.dateFrom,dateTo:form.dateTo,
   dispatchNo:form.dispatchNo,purchaseId:form.purchaseId,
   tcKimlikNo:form.tcKimlikNo,surname:form.surname,
   yabanciKimlikNo:form.yabanciKimlikNo,name:form.name,
   senderMilitaryUnitDefId:authority?.startsWith('unit:')?authority.slice(5):undefined,
   senderMilitaryOfficeDefId:authority?.startsWith('office:')?authority.slice(7):undefined,
   includeRelatedOrganizations:form.includeRelatedOrganizations,
   dispatchScope:form.dispatchScope,
   dispatchScopeCityId:form.dispatchScopeCityId??undefined,
   dispatchScopeHospitalId:form.dispatchScopeHospitalId??undefined
  };
  this.api.list(this.page(),this.pageSize(),form.search,form.includeCancelled,
    form.currentState,'','',filters)
   .pipe(finalize(()=>{if(this.lastRequest===request)this.loading.set(false);}))
   .subscribe({
    next:response=>{
     if(this.lastRequest!==request)return;
     this.records.set(response.items);
     this.total.set(response.totalCount);
    },
    error:e=>{
     if(this.lastRequest!==request)return;
     this.records.set([]);this.total.set(0);this.error(e);
    }
   });
 }
 search(){
  const values=this.searchForm.getRawValue();
  if(values.dateFrom&&values.dateTo&&values.dateFrom>values.dateTo){
   this.filterError.set('Başlangıç tarihi bitiş tarihinden sonra olamaz.');
   return;
  }
  this.filterError.set(null);
  this.appliedForm={...values};
  this.hasSearched.set(true);
  this.page.set(1);
  this.loadStatuses();
  this.load();
 }
 resetSearch(){
  this.searchForm.reset({search:'',includeCancelled:false,currentState:null,dateFrom:'',dateTo:'',
   dispatchNo:null,purchaseId:null,tcKimlikNo:'',yabanciKimlikNo:'',name:'',surname:'',
   senderAuthority:null,includeRelatedOrganizations:false,
   dispatchScope:null,dispatchScopeCityId:null,dispatchScopeHospitalId:null});
  this.appliedForm=null;
  ++this.lastRequest; // Devam eden eski bir sorgu temizlenen ekranı tekrar dolduramaz.
  this.loading.set(false);
  this.hasSearched.set(false);
  this.records.set([]);
  this.total.set(0);
  this.page.set(1);
  this.filterError.set(null);
 }
 toPage(n:number){if(!this.hasSearched()||n<1||n>this.maxPage())return;this.page.set(n);this.load();}
 setPageSize(e:Event){
  const value=Number((e.target as HTMLSelectElement).value);
  if(![10,20,50,100].includes(value))return;
  this.pageSize.set(value);
  if(this.hasSearched()){this.page.set(1);this.load();}
 }
 openCreate(){if(this.canCreate())void this.router.navigateByUrl('/app/dispatches/new');}
 openEdit(row:DispatchItem){
  if(!this.canUpdate()||row.currentState!==1)return;
  void this.router.navigate(['/app/dispatches',row.id,'edit']);
 }
 cancel(row:DispatchItem){
  if(!this.canUpdate()||row.currentState!==1)return;
  this.confirm.confirm({
   header:'Sevk İptali',
   message:row.dispatchNo+' numaralı sevk iptal edilsin mi? Kayıt geçmişte korunur.',
   acceptLabel:'İptal Et',rejectLabel:'Vazgeç',
   accept:()=>this.api.cancel(row).subscribe({
    next:()=>{this.message.add({severity:'success',summary:'Sevk iptal edildi'});this.load();},
    error:e=>this.error(e)
   })
  });
 }
 stateLabel(state:number):string{
  return this.statuses().find(item=>item.code===state)?.name??'Durum #'+state;
 }
 stateSeverity(state:number):'success'|'warn'|'danger'|'info'{
  return state===4?'danger':state===1?'success':state===3?'info':'warn';
 }
 private error(e:unknown){
  const response=e as HttpErrorResponse;
  const msg=response.error?.detail||response.error?.title||response.message||'İşlem gerçekleştirilemedi.';
  this.message.add({severity:'error',summary:'Sevk sorgusu başarısız',detail:msg,life:7500});
 }
}

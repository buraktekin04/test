import {DatePipe} from '@angular/common';
import {HttpErrorResponse} from '@angular/common/http';
import {AfterViewInit,Component,computed,inject,OnInit,signal} from '@angular/core';
import {FormBuilder,ReactiveFormsModule,FormsModule} from '@angular/forms';
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
import {DispatchItem,DispatchOption,DispatchStatusOption,DispatchAdvancedFilters,DispatchResponse,OptionType} from '../models/dispatch.model';
import {DispatchService} from '../services/dispatch.service';

/** Sevk listesi ve sunucu üzerinde sorgulama. Sevk giriş formunu içermez. */
@Component({
 selector:'app-dispatch-list',standalone:true,
 imports:[DatePipe,ReactiveFormsModule,FormsModule,ButtonModule,CheckboxModule,ConfirmDialogModule,
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
 readonly canCreateProvision=computed(()=>this.permissions.has(PermissionCodes.Provisions.CreateWithDispatch));
 readonly canAudit=computed(()=>this.permissions.has(PermissionCodes.AuditLogs.View));

 readonly records=signal<DispatchItem[]>([]);
 readonly total=signal(0);
 readonly page=signal(1);
 readonly pageSize=signal(20);
 readonly maxPage=computed(()=>Math.max(1,Math.ceil(this.total()/this.pageSize())));
 readonly loading=signal(false);
 readonly hasSearched=signal(false);
 readonly auditRecord=signal<DispatchItem|null>(null);
 readonly detail=signal<DispatchResponse|null>(null);
 readonly detailOpen=signal(false);
 readonly detailLoading=signal(false);
 readonly detailPhotoUrl=signal<string|null>(null);
 readonly detailNames=signal<Record<string,string>>({});
 readonly detailGender=signal('—');
 readonly cancelOpen=signal(false);
 readonly cancelTarget=signal<DispatchItem|null>(null);
 cancelReason='';
 readonly columns=signal<Record<string,boolean>>({
  dispatchNo:true,personName:true,identityMasked:true,dispatchDate:true,
  dispatchDocumentDate:false,dispatchDocumentNo:true,admissionTypeName:true,currentState:true
 });
 showColumn(name:string):boolean{return this.columns()[name]??true;}
 toggleColumn(name:string,checked:boolean):void{
  this.columns.update(v=>({...v,[name]:checked}));
 }
 readonly selectableColumns=[
  {name:'dispatchNo',label:'Sevk No'},
  {name:'personName',label:'Başvuran Kişi'},
  {name:'identityMasked',label:'Kimlik No (Maskeli)'},
  {name:'dispatchDate',label:'Sevk Tarihi'},
  {name:'dispatchDocumentDate',label:'Evrak Tarihi'},
  {name:'dispatchDocumentNo',label:'Evrak No'},
  {name:'admissionTypeName',label:'Başvuru Grubu'},
  {name:'currentState',label:'Durum'}
 ];
 readonly visibleColumnCount=computed(()=>Object.values(this.columns()).filter(Boolean).length);

 private lastRequest=0;
 private detailVersion=0;

 readonly statuses=signal<DispatchStatusOption[]>([]);
 readonly statusItems=computed(()=>[
  {label:'Tüm durumlar',value:null as number|null},
  ...this.statuses().map(x=>({label:x.name,value:x.code as number|null}))
 ]);
 /** Sorgulama filtresi zorunlu değildir; boş değer tüm kapsamları (tarihsel 1 dahil) gösterir. */
 readonly scopeItems=[
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
  if(values.tcKimlikNo.trim()&&!this.validTc(values.tcKimlikNo.trim())){
   this.filterError.set('Geçersiz T.C. Kimlik numarası.');return;
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
 private validTc(value:string):boolean{
  if(!/^[1-9][0-9]{10}$/.test(value))return false;
  const d=value.split('').map(Number);
  const odd=d[0]+d[2]+d[4]+d[6]+d[8];
  const even=d[1]+d[3]+d[5]+d[7];
  return ((odd*7-even)%10+10)%10===d[9] &&
   d.slice(0,10).reduce((sum,n)=>sum+n,0)%10===d[10];
 }
 toPage(n:number){if(!this.hasSearched()||n<1||n>this.maxPage())return;this.page.set(n);this.load();}
 setPageSize(e:Event){
  const value=Number((e.target as HTMLSelectElement).value);
  if(![10,20,50,100].includes(value))return;
  this.pageSize.set(value);
  if(this.hasSearched()){this.page.set(1);this.load();}
 }
 openCreate(){if(this.canCreate())void this.router.navigateByUrl('/app/dispatches/new');}
 openProvision(row:DispatchItem){
  if(!this.canCreateProvision()||row.currentState!==1)return;
  void this.router.navigate(['/app/provisions/with-dispatch'],{queryParams:{dispatchId:row.id}});
 }
 openEdit(row:DispatchItem){
  if(!this.canUpdate()||row.currentState!==1)return;
  void this.router.navigate(['/app/dispatches',row.id,'edit']);
 }
 cancel(row:DispatchItem){
  if(!this.canUpdate()||row.currentState!==1)return;
  this.cancelTarget.set(row);this.cancelReason='';this.cancelOpen.set(true);
 }
 confirmCancel():void{
  const row=this.cancelTarget();
  const reason=this.cancelReason.trim();
  if(!row||!this.canUpdate()||row.currentState!==1||reason.length===0||reason.length>500){
   this.message.add({severity:'warn',summary:'İptal açıklaması 1-500 karakter olmalıdır.'});return;
  }
  this.api.cancel(row,reason).subscribe({
   next:()=>{
    this.cancelOpen.set(false);this.cancelTarget.set(null);this.cancelReason='';
    this.message.add({severity:'success',summary:'Sevk iptal edildi'});this.load();
   },
   error:e=>this.error(e)
  });
 }

 openDetail(row:DispatchItem):void{
  if(!this.canView())return;
  this.closeDetail();
  const request=++this.detailVersion;
  this.detailOpen.set(true);this.detailLoading.set(true);
  this.api.detail(row.id).subscribe({
   next:d=>{
    if(!this.detailOpen()||request!==this.detailVersion)return;
    this.detail.set(d);this.detailLoading.set(false);this.loadDetailNames(d);
    this.detailGender.set('—');
    if(d.person.gender!=null)
     this.api.genders().subscribe({next:items=>{
      if(this.detail()?.id!==d.id)return;
      this.detailGender.set(items.find(x=>x.code===d.person.gender)?.name??'—');
     }});
    if(d.hasPhoto){
     this.api.getPhoto(d.id).subscribe({
      next:photo=>{
       if(!this.detailOpen()||request!==this.detailVersion||this.detail()?.id!==d.id)return;
       this.releaseDetailPhoto();
       this.detailPhotoUrl.set(URL.createObjectURL(photo));
      },
      error:()=>this.detailPhotoUrl.set(null)
     });
    }
   },
   error:e=>{
    if(request!==this.detailVersion)return;
    this.detailLoading.set(false);this.closeDetail();this.error(e);
   }
  });
 }
 closeDetail():void{
  ++this.detailVersion;
  this.detailOpen.set(false);this.detail.set(null);this.detailNames.set({});this.detailGender.set('—');
  this.releaseDetailPhoto();
 }
 private releaseDetailPhoto():void{
  const previous=this.detailPhotoUrl();
  if(previous)URL.revokeObjectURL(previous);
  this.detailPhotoUrl.set(null);
 }
 detailName(key:string):string{return this.detailNames()[key]??'—';}
 private loadDetailNames(d:DispatchResponse):void{
  const ids:{key:string;type:OptionType;id:string|null|undefined}[]=[
   {key:'nationality',type:'countries',id:d.person.nationalityId},
   {key:'town',type:'towns',id:d.person.townOfBirthId},
   {key:'admission',type:'admissionTypes',id:d.dispatch.hcAdmissionTypeDefId},
   {key:'reason',type:'reasons',id:d.dispatch.reasonForExaminationDefId},
   {key:'forces',type:'forces',id:d.dispatch.forcesCommandDefId},
   {key:'rank',type:'ranks',id:d.dispatch.rankDefId},
   {key:'class',type:'classes',id:d.dispatch.militaryClassDefId},
   {key:'unit',type:'units',id:d.dispatch.militaryUnitDefId},
   {key:'school',type:'schools',id:d.dispatch.militarySchoolDefId},
   {key:'office',type:'offices',id:d.dispatch.militaryOfficeDefId},
   {key:'senderUnit',type:'units',id:d.dispatch.senderMilitaryUnitDefId},
   {key:'senderOffice',type:'offices',id:d.dispatch.senderMilitaryOfficeDefId},
   {key:'mission',type:'missions',id:d.dispatch.flyingMissionDefId},
   {key:'plane',type:'planes',id:d.dispatch.planeTypeDefId},
   {key:'scopeCity',type:'cities',id:d.dispatch.dispatchScopeCityId},
   {key:'scopeHospital',type:'hospitals',id:d.dispatch.dispatchScopeHospitalId}
  ];
  for(const item of ids){
   if(!item.id)continue;
   this.api.options(item.type,'',item.id).subscribe({
    next:opts=>{
     if(this.detail()?.id!==d.id)return;
     const value=opts.find(x=>x.id===item.id)?.label;
     if(value)this.detailNames.update(v=>({...v,[item.key]:value}));
    }
   });
  }
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

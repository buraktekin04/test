import {Component,inject,OnInit,signal,computed} from '@angular/core';
import {PermissionCodes} from '../../../core/permissions/constants/permission-codes';
import {PermissionService} from '../../../core/permissions/services/permission.service';
import {DatePipe} from '@angular/common';
import {ReactiveFormsModule,FormBuilder} from '@angular/forms';
import {ActivatedRoute,RouterLink} from '@angular/router';
import {FormsModule} from '@angular/forms';
import {ButtonModule} from 'primeng/button';
import {TableModule} from 'primeng/table';
import {SelectModule} from 'primeng/select';
import {DialogModule} from 'primeng/dialog';
import {TagModule} from 'primeng/tag';
import {CheckboxModule} from 'primeng/checkbox';
import {AuthService} from '../../../core/auth/services/auth.service';
import type {OptionType,DispatchPerson,DispatchDetails} from '../../dispatches/models/dispatch.model';
import {DispatchService} from '../../dispatches/services/dispatch.service';
import type {DispatchOption} from '../../dispatches/models/dispatch.model';
import {ProvisionService} from '../services/provision.service';
import type {ProvisionItem,ProvisionDetail,ProvisionFilter,ProvisionStatusOption} from '../models/provision.model';

@Component({
 selector:'app-provision-query',standalone:true,
 imports:[ReactiveFormsModule,FormsModule,RouterLink,DatePipe,ButtonModule,TableModule,
  SelectModule,DialogModule,TagModule,CheckboxModule],
 templateUrl:'./provision-query.component.html',
 styleUrl:'./provision-query.component.scss'
})
export class ProvisionQueryComponent implements OnInit {
 private readonly api=inject(ProvisionService);
 private readonly dispatchApi=inject(DispatchService);
 private readonly route=inject(ActivatedRoute);
 private readonly fb=inject(FormBuilder);
 private readonly permissions=inject(PermissionService);
 private readonly auth=inject(AuthService);
 readonly canCancel=computed(()=>this.permissions.has(PermissionCodes.Provisions.Cancel));
 readonly hospitals=signal<DispatchOption[]>([]);
 readonly statuses=signal<ProvisionStatusOption[]>([]);
 readonly detailHospitalName=signal('');
 readonly detailNames=signal<Record<string,string>>({});
 readonly detailGender=signal('—');
 readonly canIncludeRelatedOrganizations=computed(()=>
  !!this.auth.currentUser()?.militaryUnitDefId);
 readonly selectableColumns=[
  {key:'provisionNo',label:'Provizyon No'},
  {key:'dispatchNo',label:'Sevk No'},
  {key:'provisionDate',label:'Provizyon Tarihi'},
  {key:'hospitalName',label:'Hastane'},
  {key:'personName',label:'Ad Soyad'},
  {key:'identityMasked',label:'Kimlik (Maskeli)'},
  {key:'currentState',label:'Durum'},
  {key:'withoutDispatch',label:'Tür'},
  {key:'purchaseId',label:'Alım ID'},
  {key:'description',label:'Açıklama'}
 ];
 readonly columns=signal<Record<string,boolean>>({
  provisionNo:true,dispatchNo:true,provisionDate:true,hospitalName:true,
  personName:true,identityMasked:true,currentState:true,withoutDispatch:true,
  purchaseId:false,description:false
 });
 readonly showColumn=(name:string):boolean=>this.columns()[name]??true;
 readonly toggleColumn=(name:string,checked:boolean):void=>
  this.columns.update(x=>({...x,[name]:checked}));
 readonly visibleColumnCount=computed(()=>Object.values(this.columns()).filter(Boolean).length);
 readonly records=signal<ProvisionItem[]>([]);
 readonly totalCount=signal(0);
 readonly page=signal(1);
 readonly pageSize=20;
 readonly searched=signal(false);
 readonly loading=signal(false);
 readonly error=signal('');
 readonly notice=signal('');
 readonly detail=signal<ProvisionDetail|null>(null);
 readonly detailOpen=signal(false);
 readonly cancelOpen=signal(false);
 readonly selected=signal<ProvisionItem|null>(null);
 private detailVersion=0;
 cancelReason='';
 readonly form=this.fb.nonNullable.group({
  dateFrom:'',dateTo:'',currentState:0,provisionNo:0,hospitalId:'',
  tcKimlikNo:'',yabanciKimlikNo:'',name:'',surname:'',purchaseId:0,
  includeRelatedOrganizations:false
 });
 ngOnInit():void{
  const no=this.route.snapshot.queryParamMap.get('provisionNo');
  if(no&&/^\d+$/.test(no))this.form.patchValue({provisionNo:Number(no)});
  this.dispatchApi.options('hospitals').subscribe({
   next:items=>this.hospitals.set(items),
   error:()=>this.error.set('Hastaneler yüklenemedi.')
  });
  this.api.statuses().subscribe({
   next:statuses=>this.statuses.set(statuses),
   error:()=>this.error.set('Provizyon durum tanımları yüklenemedi.')
  });
  if(no)this.search();
 }
 stateLabel(state:number):string{
  return this.statuses().find(x=>x.code===state)?.name??'Durum '+state;
 }
 private filters():ProvisionFilter{
  const v=this.form.getRawValue();
  return {
   dateFrom:v.dateFrom||undefined,dateTo:v.dateTo||undefined,
   currentState:v.currentState||undefined,
   provisionNo:v.provisionNo||undefined,hospitalId:v.hospitalId||undefined,
   tcKimlikNo:v.tcKimlikNo.trim()||undefined,
   yabanciKimlikNo:v.yabanciKimlikNo.trim()||undefined,
   name:v.name.trim()||undefined,surname:v.surname.trim()||undefined,
   purchaseId:v.purchaseId||undefined,
   includeRelatedOrganizations:v.includeRelatedOrganizations||undefined
  };
 }
 search(newPage=1):void{
  if(this.loading())return;
  const v=this.form.getRawValue();
  if(v.dateFrom&&v.dateTo&&v.dateFrom>v.dateTo){
   this.error.set('Bitiş tarihi başlangıç tarihinden önce olamaz.');return;
  }
  if(v.tcKimlikNo.trim()&&!this.validTc(v.tcKimlikNo.trim())){
   this.error.set('Geçersiz T.C. Kimlik numarası.');return;
  }
  this.page.set(newPage);this.loading.set(true);this.error.set('');this.notice.set('');
  this.api.list(this.filters(),this.page(),this.pageSize).subscribe({
   next:res=>{
    this.records.set(res.items);this.totalCount.set(res.totalCount);
    this.searched.set(true);this.loading.set(false);
   },
   error:e=>{this.loading.set(false);this.error.set(e.error?.detail||'Provizyon sorgulaması başarısız.');}
  });
 }
 private validTc(value:string):boolean{
  if(!/^[1-9][0-9]{10}$/.test(value))return false;
  const d=value.split('').map(Number);
  const odd=d[0]+d[2]+d[4]+d[6]+d[8];
  const even=d[1]+d[3]+d[5]+d[7];
  return ((odd*7-even)%10+10)%10===d[9] &&
   d.slice(0,10).reduce((sum,n)=>sum+n,0)%10===d[10];
 }
 openDetail(row:ProvisionItem):void{
  const request=++this.detailVersion;
  this.detail.set(null);this.detailNames.set({});this.detailGender.set('—');
  this.detailHospitalName.set(row.hospitalName);this.detailOpen.set(true);
  this.api.detail(row.id).subscribe({
   next:data=>{
    if(!this.detailOpen()||request!==this.detailVersion)return;
    this.detail.set(data);this.loadDetailNames(data.person,data.dispatch,row.id);
   },
   error:e=>{
    if(request!==this.detailVersion)return;
    this.error.set(e.error?.detail||'Provizyon detayı alınamadı.');this.closeDetail();
   }
  });
 }
 closeDetail():void{
  ++this.detailVersion;
  this.detailOpen.set(false);this.detail.set(null);
  this.detailNames.set({});this.detailGender.set('—');
 }

 /** Yanıtın mevcut GUID tanımlarını ekranda ad olarak çözer, GUID yazmaz. */
 detailName(key:string):string{return this.detailNames()[key]??'—';}
 latestStateDate():string|null{
  const states=this.detail()?.states??[];
  return states.length?states[states.length-1].createdDate:null;
 }
 private loadDetailNames(person:DispatchPerson,dispatch:DispatchDetails,provisionId:string):void{
  if(person.gender!=null)
   this.dispatchApi.genders().subscribe({
    next:types=>{
     if(this.detail()?.id===provisionId)
      this.detailGender.set(types.find(x=>x.code===person.gender)?.name??'—');
    }
   });
  const definitions:{key:string;type:OptionType;id:string|null|undefined}[]=[
   {key:'nationality',type:'countries',id:person.nationalityId},
   {key:'town',type:'towns',id:person.townOfBirthId},
   {key:'admission',type:'admissionTypes',id:dispatch.hcAdmissionTypeDefId},
   {key:'reason',type:'reasons',id:dispatch.reasonForExaminationDefId},
   {key:'forces',type:'forces',id:dispatch.forcesCommandDefId},
   {key:'rank',type:'ranks',id:dispatch.rankDefId},
   {key:'class',type:'classes',id:dispatch.militaryClassDefId},
   {key:'unit',type:'units',id:dispatch.militaryUnitDefId},
   {key:'school',type:'schools',id:dispatch.militarySchoolDefId},
   {key:'office',type:'offices',id:dispatch.militaryOfficeDefId},
   {key:'senderUnit',type:'units',id:dispatch.senderMilitaryUnitDefId},
   {key:'senderOffice',type:'offices',id:dispatch.senderMilitaryOfficeDefId},
   {key:'mission',type:'missions',id:dispatch.flyingMissionDefId},
   {key:'plane',type:'planes',id:dispatch.planeTypeDefId}
  ];
  for(const def of definitions){
   if(!def.id)continue;
   this.dispatchApi.options(def.type,'',def.id).subscribe({
    next:opts=>{
     if(this.detail()?.id!==provisionId)return;
     const name=opts.find(x=>x.id===def.id)?.label;
     if(name)this.detailNames.update(prev=>({...prev,[def.key]:name}));
    }
   });
  }
 }

 beginCancel(row:ProvisionItem):void{
  if(!this.canCancel()||row.currentState!==1)return;
  this.selected.set(row);this.cancelReason='';this.cancelOpen.set(true);
 }
 cancel():void{
  const item=this.selected();
  if(!item||!this.cancelReason.trim()){
   this.error.set('İptal açıklaması zorunludur.');return;
  }
  this.loading.set(true);this.error.set('');
  this.api.cancel(item,this.cancelReason.trim()).subscribe({
   next:()=>{this.loading.set(false);this.cancelOpen.set(false);this.selected.set(null);
    this.notice.set('Provizyon iptal edildi.');this.search(this.page());},
   error:e=>{this.loading.set(false);this.error.set(e.error?.detail||'İptal gerçekleştirilemedi.');}
  });
 }
}

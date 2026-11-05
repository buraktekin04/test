import {Component,inject,OnInit,signal,computed} from '@angular/core';
import {FormBuilder,ReactiveFormsModule,Validators} from '@angular/forms';
import {RouterLink} from '@angular/router';
import {ButtonModule} from 'primeng/button';
import {SelectModule} from 'primeng/select';
import {DispatchService} from '../../dispatches/services/dispatch.service';
import type {DispatchAdmissionRules,DispatchDetails,DispatchGenderOption,DispatchOption,
 DispatchPerson,OptionType} from '../../dispatches/models/dispatch.model';
import {ProvisionService} from '../services/provision.service';
import type {ProvisionMutation} from '../models/provision.model';

@Component({
 selector:'app-provision-without-dispatch',standalone:true,
 imports:[RouterLink,ReactiveFormsModule,ButtonModule,SelectModule],
 templateUrl:'./provision-without-dispatch.component.html',
 styleUrl:'../provision-common.scss'
})
export class ProvisionWithoutDispatchComponent implements OnInit {
 private readonly fb=inject(FormBuilder);
 private readonly provisionApi=inject(ProvisionService);
 private readonly dispatchApi=inject(DispatchService);
 readonly options=signal<Partial<Record<OptionType,DispatchOption[]>>>({});
 readonly genders=signal<DispatchGenderOption[]>([]);
 readonly rules=signal<DispatchAdmissionRules|null>(null);
 readonly loading=signal(false);
 readonly querying=signal(false);
 readonly error=signal('');
 readonly warning=signal('');
 readonly result=signal<ProvisionMutation|null>(null);
 readonly availableReasons=computed(()=>this.getOptions('reasons').filter(
  x=>!this.rules()?.unusedReasonIds?.includes(x.id)));
 private photo:File|null=null;
 readonly form=this.fb.nonNullable.group({
  tcKimlikNo:'',yabanciKimlikNo:'',
  name:['',Validators.required],surname:['',Validators.required],
  motherName:'',fatherName:'',nationalityId:'',townOfBirthId:'',
  birthDate:['',Validators.required],gender:0,phoneNumber:'',address:'',
  hospitalId:['',Validators.required],hcAdmissionTypeDefId:['',Validators.required],
  reasonForExaminationDefId:['',Validators.required],
  senderChoice:['',Validators.required],
  nthAdmission:0,dispatchDocumentDate:'',dispatchDocumentNo:'',
  forcesCommandDefId:'',rankDefId:'',militaryClassDefId:'',militaryUnitDefId:'',
  militarySchoolDefId:'',militaryOfficeDefId:'',flyingMissionDefId:'',
  planeTypeDefId:'',registryNumber:'',lastNominationDate:'',
  militaryUnitDescription:'',senderChairOther:'',purchaseId:0
 });
 readonly types:OptionType[]=['countries','towns','hospitals','admissionTypes','reasons',
  'forces','ranks','classes','units','schools','offices','senders','missions','planes'];
 ngOnInit():void{
  for(const type of this.types){
   this.dispatchApi.options(type).subscribe({
    next:items=>{
    const values=type==='senders'
     ?items.filter(x=>x.kind==='unit'||x.kind==='office')
       .map(x=>({...x,id:x.kind+':'+x.id}))
     :items;
    this.options.update(old=>({...old,[type]:values}));
   },
    error:()=>this.warning.set('Bazı tanım listeleri yüklenemedi; bağlantıyı kontrol ediniz.')
   });
  }
  this.dispatchApi.genders().subscribe({
   next:data=>this.genders.set(data),
   error:()=>this.warning.set('Cinsiyet tanımları yüklenemedi.')
  });
  this.form.controls.hcAdmissionTypeDefId.valueChanges.subscribe(id=>{
   this.rules.set(null);
   this.form.controls.reasonForExaminationDefId.setValue('');
   if(!id)return;
   this.dispatchApi.admissionRules(id).subscribe({
    next:rules=>this.rules.set(rules),
    error:()=>this.error.set('Başvuru grubu kuralları yüklenemedi.')
   });
  });
 }
 getOptions(type:OptionType):DispatchOption[]{return this.options()[type]??[];}
 onPhoto(event:Event):void{
  const input=event.target as HTMLInputElement;
  const file=input.files?.[0];
  if(!file){this.photo=null;return;}
  if(!['image/png','image/jpeg'].includes(file.type)||file.size>2*1024*1024){
   this.error.set('Fotoğraf JPEG/PNG olmalı ve 2 MiB sınırını aşmamalıdır.');
   input.value='';this.photo=null;return;
  }
  this.photo=file;this.error.set('');
 }
 lookupPerson():void{
  const v=this.form.getRawValue();
  if((!v.tcKimlikNo&&!v.yabanciKimlikNo)||!v.birthDate){
   this.error.set('Nüfus sorgusu için kimlik ve doğum tarihi giriniz.');return;
  }
  this.querying.set(true);this.error.set('');
  this.dispatchApi.queryPopulation({
   tcKimlikNo:v.tcKimlikNo||null,yabanciKimlikNo:v.yabanciKimlikNo||null,
   birthDate:v.birthDate
  }).subscribe({
   next:result=>{
    const person=result.person;
    this.form.patchValue({
     name:person.name,surname:person.surname,motherName:person.motherName??'',
     fatherName:person.fatherName??'',nationalityId:person.nationalityId??'',
     townOfBirthId:person.townOfBirthId??'',gender:person.gender??0,
     phoneNumber:person.phoneNumber??'',address:person.address??''
    });
    this.querying.set(false);
   },
   error:e=>{
    this.querying.set(false);
    this.error.set(e.status===503?'Kurum nüfus entegrasyonu henüz bağlı değil. Kişi bilgilerini elle girebilirsiniz.':
     e.error?.detail||'Nüfus sorgusu yapılamadı. Kişi bilgilerini elle girebilirsiniz.');
   }
  });
 }
 private maybe(s:string|null):string|null{return (s??'').trim()||null;}
 private num(n:number):number|null{return n>0?n:null;}
 private encodePhoto(file:File):Promise<string>{
  return new Promise((resolve,reject)=>{
   const reader=new FileReader();
   reader.onload=()=>{
    const data=String(reader.result??'');
    const separator=data.indexOf(',');
    if(separator<0){reject(new Error('Fotoğraf okunamadı.'));return;}
    resolve(data.slice(separator+1));
   };
   reader.onerror=()=>reject(new Error('Fotoğraf okunamadı.'));
   reader.readAsDataURL(file);
  });
 }
 async save():Promise<void>{
  if(this.loading()||this.form.invalid)return;
  this.error.set('');this.warning.set('');
  const v=this.form.getRawValue();
  if(!!v.tcKimlikNo===!!v.yabanciKimlikNo){
   this.error.set('T.C. veya yabancı kimlik numarasından yalnızca biri dolu olmalıdır.');return;
  }
  if(!this.rules()){
   this.error.set('Başvuru grubu kuralları yüklenmeden kayıt yapılmaz.');return;
  }
  if(this.rules()?.unusedReasonIds.includes(v.reasonForExaminationDefId)){
   this.error.set('Bu başvuru grubunda seçilen muayene sebebi kullanılamaz.');return;
  }
  const sender=/^(unit|office):([0-9a-f-]{36})$/i.exec(v.senderChoice);
  if(!sender){
   this.error.set('Muayeneye gönderen makamın birlik veya askerlik şubesi türünü seçiniz.');return;
  }
  const person:DispatchPerson={
   tcKimlikNo:this.maybe(v.tcKimlikNo),yabanciKimlikNo:this.maybe(v.yabanciKimlikNo),
   name:v.name.trim(),surname:v.surname.trim(),
   motherName:this.maybe(v.motherName),fatherName:this.maybe(v.fatherName),
   nationalityId:this.maybe(v.nationalityId),townOfBirthId:this.maybe(v.townOfBirthId),
   birthDate:v.birthDate,gender:this.num(v.gender),
   phoneNumber:this.maybe(v.phoneNumber),address:this.maybe(v.address),
   birthPlaceText:null,birthPlaceCode:null
  };
  const dispatch:DispatchDetails={
   nthAdmission:this.num(v.nthAdmission),dispatchDocumentDate:this.maybe(v.dispatchDocumentDate),
   dispatchDocumentNo:this.maybe(v.dispatchDocumentNo),
   reasonForExaminationDefId:v.reasonForExaminationDefId,
   // Sevksiz provizyonda kapsam seçtirilmez; backend kabul hastanesini kapsam olarak sabitler.
   dispatchScope:3,dispatchScopeCityId:null,dispatchScopeHospitalId:v.hospitalId,
   hcAdmissionTypeDefId:v.hcAdmissionTypeDefId,
   forcesCommandDefId:this.maybe(v.forcesCommandDefId),
   rankDefId:this.maybe(v.rankDefId),
   militaryClassDefId:this.maybe(v.militaryClassDefId),
   militaryUnitDefId:this.maybe(v.militaryUnitDefId),
   militarySchoolDefId:this.maybe(v.militarySchoolDefId),
   senderMilitaryUnitDefId:sender[1].toLowerCase()==='unit'?sender[2]:null,
   senderMilitaryOfficeDefId:sender[1].toLowerCase()==='office'?sender[2]:null,
   senderChairOther:this.maybe(v.senderChairOther),
   militaryOfficeDefId:this.maybe(v.militaryOfficeDefId),
   flyingMissionDefId:this.maybe(v.flyingMissionDefId),
   planeTypeDefId:this.maybe(v.planeTypeDefId),
   registryNumber:this.maybe(v.registryNumber),
   lastNominationDate:this.maybe(v.lastNominationDate),
   militaryUnitDescription:this.maybe(v.militaryUnitDescription),
   purchaseId:this.num(v.purchaseId),description:null
  };
  this.loading.set(true);
  let photoBase64:string|null=null;
  if(this.photo){
   try{photoBase64=await this.encodePhoto(this.photo);}
   catch{
    this.loading.set(false);this.error.set('Fotoğraf okunamadı; tekrar seçiniz.');return;
   }
  }
  this.provisionApi.withoutDispatch(v.hospitalId,person,dispatch,null,photoBase64).subscribe({
   next:saved=>{
    this.result.set(saved);
    this.loading.set(false);
   },
   error:e=>{this.loading.set(false);this.error.set(e.error?.detail||e.error?.title||
    'Sevksiz provizyon kaydedilemedi.');}
  });
 }
}

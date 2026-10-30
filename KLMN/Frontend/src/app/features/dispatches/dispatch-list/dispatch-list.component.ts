import {Component,computed,inject,OnDestroy,OnInit,signal} from '@angular/core';
import {DatePipe} from '@angular/common';
import {HttpErrorResponse} from '@angular/common/http';
import {FormBuilder,ReactiveFormsModule,Validators} from '@angular/forms';
import {finalize,firstValueFrom} from 'rxjs';
import {ConfirmationService,MessageService} from 'primeng/api';
import {ButtonModule} from 'primeng/button';
import {CheckboxModule} from 'primeng/checkbox';
import {ConfirmDialogModule} from 'primeng/confirmdialog';
import {DialogModule} from 'primeng/dialog';
import {InputTextModule} from 'primeng/inputtext';
import {InputNumberModule} from 'primeng/inputnumber';
import {SelectModule} from 'primeng/select';
import {TableModule} from 'primeng/table';
import {TagModule} from 'primeng/tag';
import {ToastModule} from 'primeng/toast';
import {PermissionCodes} from '../../../core/permissions/constants/permission-codes';
import {PermissionService} from '../../../core/permissions/services/permission.service';
import {AuditHistoryComponent} from '../../../shared/audit-history/audit-history.component';
import {DispatchService} from '../services/dispatch.service';
import {DispatchQrReaderComponent} from '../qr/dispatch-qr-reader.component';
import {DispatchPhotoCaptureComponent} from '../camera/dispatch-photo-capture.component';
import {DispatchQrData,DispatchQrFormatError,parseDispatchQr} from '../qr/dispatch-qr.parser';
import {DispatchItem,DispatchOption,DispatchPerson,DispatchDetails,DispatchResponse,
 DispatchFileItem,OptionType,DispatchSaveResponse,DispatchAdmissionRules} from '../models/dispatch.model';

/** Eski sevk ekranının DevExtreme bağımlılığı taşımayan, bağımsız Angular 21 / PrimeNG karşılığı. */
@Component({
 selector:'app-dispatch-list',standalone:true,
 imports:[DatePipe,ReactiveFormsModule,ButtonModule,CheckboxModule,ConfirmDialogModule,
  DialogModule,InputTextModule,InputNumberModule,SelectModule,TableModule,TagModule,ToastModule,
  AuditHistoryComponent,DispatchQrReaderComponent,DispatchPhotoCaptureComponent],
 providers:[ConfirmationService,MessageService],
 templateUrl:'./dispatch-list.component.html',
 styleUrl:'./dispatch-list.component.scss'
})
export class DispatchListComponent implements OnInit,OnDestroy {
 private readonly api=inject(DispatchService);
 private readonly fb=inject(FormBuilder);
 private readonly permissions=inject(PermissionService);
 private readonly confirm=inject(ConfirmationService);
 private readonly message=inject(MessageService);
 readonly canView=computed(()=>this.permissions.has(PermissionCodes.Dispatches.View)||this.permissions.has(PermissionCodes.Dispatches.Query));
 readonly canQuery=computed(()=>this.permissions.has(PermissionCodes.Dispatches.Query));
 readonly canCreate=computed(()=>this.permissions.has(PermissionCodes.Dispatches.Create));
 readonly canUpdate=computed(()=>this.permissions.has(PermissionCodes.Dispatches.Update));
 readonly canAudit=computed(()=>this.permissions.has(PermissionCodes.AuditLogs.View));
 readonly records=signal<DispatchItem[]>([]);
 readonly total=signal(0);
 readonly page=signal(1);
 readonly pageSize=signal(20);
 readonly maxPage=computed(()=>Math.max(1,Math.ceil(this.total()/this.pageSize())));
 readonly loading=signal(false);
 readonly saving=signal(false);
 readonly queryingPerson=signal(false);
 readonly dialogVisible=signal(false);
 readonly editing=signal<DispatchResponse|null>(null);
 readonly auditRecord=signal<DispatchItem|null>(null);
 readonly files=signal<DispatchFileItem[]>([]);
 readonly formError=signal<string|null>(null);
 readonly identityType=signal<'tc'|'foreign'>('tc');
 readonly searchForm=this.fb.nonNullable.group({search:'',includeCancelled:false});
 readonly personForm=this.fb.group({
  tcKimlikNo:this.fb.control<string|null>(null),
  yabanciKimlikNo:this.fb.control<string|null>(null),
  name:this.fb.nonNullable.control('',[Validators.required,Validators.maxLength(64)]),
  surname:this.fb.nonNullable.control('',[Validators.required,Validators.maxLength(64)]),
  motherName:this.fb.control<string|null>(null),
  fatherName:this.fb.control<string|null>(null),
  nationalityId:this.fb.control<string|null>(null),
  townOfBirthId:this.fb.control<string|null>(null),
  birthDate:this.fb.nonNullable.control('',Validators.required),
  gender:this.fb.control<number|null>(null),
  phoneNumber:this.fb.control<string|null>(null),
  address:this.fb.control<string|null>(null),
  birthPlaceText:this.fb.control<string|null>(null),
  birthPlaceCode:this.fb.control<string|null>(null)
 });
 readonly dispatchForm=this.fb.group({
  nthAdmission:this.fb.control<number|null>(null),
  dispatchDocumentDate:this.fb.control<string|null>(null),
  dispatchDocumentNo:this.fb.control<string|null>(null),
  reasonForExaminationDefId:this.fb.control<string|null>(null,Validators.required),
  dispatchScope:this.fb.nonNullable.control(1),
  dispatchScopeCityId:this.fb.control<string|null>(null),
  dispatchScopeHospitalId:this.fb.control<string|null>(null),
  hcAdmissionTypeDefId:this.fb.control<string|null>(null,Validators.required),
  forcesCommandDefId:this.fb.control<string|null>(null),
  rankDefId:this.fb.control<string|null>(null),
  militaryClassDefId:this.fb.control<string|null>(null),
  militaryUnitDefId:this.fb.control<string|null>(null),
  militarySchoolDefId:this.fb.control<string|null>(null),
  senderMilitaryUnitDefId:this.fb.control<string|null>(null),
  senderMilitaryOfficeDefId:this.fb.control<string|null>(null),
  senderChairOther:this.fb.control<string|null>(null),
  militaryOfficeDefId:this.fb.control<string|null>(null),
  flyingMissionDefId:this.fb.control<string|null>(null),
  planeTypeDefId:this.fb.control<string|null>(null),
  registryNumber:this.fb.control<string|null>(null),
  lastNominationDate:this.fb.control<string|null>(null),
  militaryUnitDescription:this.fb.control<string|null>(null),
  purchaseId:this.fb.control<number|null>(null),
  description:this.fb.control<string|null>(null)
 });
 readonly form=this.fb.group({person:this.personForm,dispatch:this.dispatchForm});
 readonly scopeItems=[{label:'Kapsam belirtilmemiş',value:1},
  {label:'Şehirdeki sağlık tesisleri',value:2},{label:'Belirli sağlık tesisi',value:3}];
 readonly genderItems=[{label:'Belirtilmedi',value:null},{label:'Erkek (1)',value:1},{label:'Kadın (2)',value:2}];
 readonly personTexts=[
  {key:'name',label:'Adı *'},{key:'surname',label:'Soyadı *'},
  {key:'motherName',label:'Anne Adı'},{key:'fatherName',label:'Baba Adı'},
  {key:'phoneNumber',label:'Telefon No'},{key:'address',label:'Adres'}
 ];
 readonly personSelections=[
  {key:'nationalityId',label:'Uyruğu',type:'countries' as OptionType},
  {key:'townOfBirthId',label:'Doğum Yeri',type:'towns' as OptionType}
 ];
 readonly dispatchSelections=[
  {key:'hcAdmissionTypeDefId',label:'Başvuru Grubu *',type:'admissionTypes' as OptionType},
  {key:'reasonForExaminationDefId',label:'Muayene Sebebi *',type:'reasons' as OptionType},
  {key:'forcesCommandDefId',label:'Kuvvet',type:'forces' as OptionType},
  {key:'rankDefId',label:'Rütbesi',type:'ranks' as OptionType},
  {key:'militaryClassDefId',label:'Sınıfı',type:'classes' as OptionType},
  {key:'militaryUnitDefId',label:'Birliği',type:'units' as OptionType},
  {key:'militarySchoolDefId',label:'Okulu',type:'schools' as OptionType},
  {key:'militaryOfficeDefId',label:'Askerlik Şubesi',type:'offices' as OptionType},
  {key:'senderMilitaryUnitDefId',label:'Gönderen Birlik',type:'units' as OptionType},
  {key:'senderMilitaryOfficeDefId',label:'Gönderen Askerlik Şubesi',type:'offices' as OptionType},
  {key:'flyingMissionDefId',label:'Uçucu Görev',type:'missions' as OptionType},
  {key:'planeTypeDefId',label:'Uçtuğu Uçak Tipi',type:'planes' as OptionType}
 ];
 readonly optionMap=signal<Record<OptionType,DispatchOption[]>>({
  countries:[],towns:[],cities:[],hospitals:[],admissionTypes:[],reasons:[],
  forces:[],ranks:[],classes:[],units:[],schools:[],offices:[],missions:[],planes:[]
 });
 private optionRequest:Partial<Record<OptionType,number>>={};
 /** Başvuru grubuna göre sunucuda tanımlanmış zorunluluklar. */
 readonly admissionRules=signal<DispatchAdmissionRules|null>(null);
 private rulesRequest=0;
 /** Kameradan çekilen veya bilgisayardan seçilen, sevk kaydından bağımsız bekleyen fotoğraf. */
 readonly photoFile=signal<File|null>(null);
 readonly photoUrl=signal<string|null>(null);
 readonly qrVisible=signal(false);
 private photoObjectUrl:string|null=null;
 private photoRequest=0;
 readonly uploadFile=signal<File|null>(null);
 private lastRequest=0;
 private editRequest=0;

 ngOnInit(){
  this.dispatchForm.controls.hcAdmissionTypeDefId.valueChanges.subscribe(id=>{
   this.dispatchForm.controls.reasonForExaminationDefId.setValue(null);
   if(!id){++this.rulesRequest;this.admissionRules.set(null);return;}
   this.loadAdmissionRules(id);
  });
  this.dispatchForm.controls.dispatchScope.valueChanges.subscribe(scope=>{
   if(scope!==2)this.dispatchForm.controls.dispatchScopeCityId.setValue(null);
   if(scope!==3)this.dispatchForm.controls.dispatchScopeHospitalId.setValue(null);
  });
  if(this.canView())this.load();
  // Filtreleme sunucu tarafında yapılır, ilk 100 sonuç için yerel cache tutulur.
  for(const type of Object.keys(this.optionMap()) as OptionType[])this.loadOptions(type);
 }
 /** Tanım aramaları istek sıralamasıyla korunur; eski cevap yeni arama sonuçlarını ezmez. */
 loadOptions(type:OptionType,search='',selectedId?:string){
  const request=(this.optionRequest[type]??0)+1;
  this.optionRequest[type]=request;
  this.api.options(type,search,selectedId).subscribe({
   next:values=>{if(this.optionRequest[type]===request){
    if(type==='reasons')values=values.filter(x=>!this.admissionRules()?.unusedReasonIds.includes(x.id));
    const prev=this.optionMap()[type];
    const selected=selectedId?prev.find(x=>x.id===selectedId):undefined;
    this.optionMap.update(m=>({...m,[type]:selected&&!values.some(x=>x.id===selected.id)?[selected,...values]:values}));
   }},
   error:()=>{if(this.optionRequest[type]===request)this.optionMap.update(m=>({...m,[type]:[]}));}
  });
 }
 /** Değişen başvuru grubu için eski seçimi doğrular ve yasaklanan muayene sebeplerini eler. */
 private loadAdmissionRules(id:string){
  const request=++this.rulesRequest;
  this.api.admissionRules(id).subscribe({
   next:rules=>{if(this.rulesRequest!==request)return;
    this.admissionRules.set(rules);
    const selected=this.dispatchForm.controls.reasonForExaminationDefId.value;
    if(selected&&rules.unusedReasonIds.includes(selected))this.dispatchForm.controls.reasonForExaminationDefId.setValue(null);
    this.loadOptions('reasons');
   },
   error:e=>{if(this.rulesRequest!==request)return;this.admissionRules.set(null);this.error(e);}
  });
 }
 /** Kaynağın zorunlu bayraklarını UI üzerinde alan etiketine yansıtır. */
 isRequired(key:string):boolean{
  const r=this.admissionRules();
  if(!r)return false;
  const flags:Record<string,boolean>={
   forcesCommandDefId:r.forcesCommandRequired,
   rankDefId:r.rankRequired,militaryClassDefId:r.militaryClassRequired,
   militaryUnitDefId:r.militaryUnitRequired,militaryOfficeDefId:r.militaryOfficeRequired,
   militarySchoolDefId:r.militarySchoolRequired
  };
  return flags[key]??false;
 }
 filterOptions(type:OptionType,filter:string){this.loadOptions(type,filter??'',this.selectedId(type));}
 private selectedId(type:OptionType):string|undefined {
  const keys=[...this.personSelections,...this.dispatchSelections];
  const field=keys.find(x=>x.type===type);
  if(!field)return undefined;
  return (this.personForm.get(field.key)?.value||this.dispatchForm.get(field.key)?.value||undefined) as string|undefined;
 }
 /** Filtre/sayfalama backend, kolon sıralaması PrimeNG p-table üzerinde yapılır. */
 load(){
  if(!this.canView())return;
  const request=++this.lastRequest;this.loading.set(true);
  const f=this.searchForm.getRawValue();
  this.api.list(this.page(),this.pageSize(),f.search,f.includeCancelled)
   .pipe(finalize(()=>{if(this.lastRequest===request)this.loading.set(false);}))
   .subscribe({next:r=>{if(this.lastRequest!==request)return;
    this.records.set(r.items);this.total.set(r.totalCount);},
    error:e=>{if(this.lastRequest!==request)return;this.records.set([]);this.total.set(0);this.error(e);}});
 }
 search(){this.page.set(1);this.load();}
 resetSearch(){this.searchForm.reset({search:'',includeCancelled:false});this.search();}
 toPage(n:number){if(n<1||n>this.maxPage())return;this.page.set(n);this.load();}
 setPageSize(e:Event){const value=Number((e.target as HTMLSelectElement).value);
  if(![10,20,50,100].includes(value))return;this.pageSize.set(value);this.search();}
 setIdentity(kind:'tc'|'foreign'){
  this.identityType.set(kind);
  this.personForm.patchValue({tcKimlikNo:null,yabanciKimlikNo:null});
 }
 openCreate(){
  if(!this.canCreate())return;
  ++this.editRequest;this.editing.set(null);this.files.set([]);
  this.formError.set(null);this.uploadFile.set(null);this.identityType.set('tc');
  this.resetPhoto();
  this.personForm.reset({name:'',surname:'',birthDate:'',tcKimlikNo:null,yabanciKimlikNo:null});
  this.dispatchForm.reset({dispatchScope:1,reasonForExaminationDefId:null,hcAdmissionTypeDefId:null});
  this.dialogVisible.set(true);
 }
 openEdit(row:DispatchItem){
  if(!this.canUpdate()||row.currentState!==1)return;
  const id=++this.editRequest;
  this.api.detail(row.id).subscribe({next:item=>{
   if(this.editRequest!==id)return;
   this.editing.set(item);this.formError.set(null);this.uploadFile.set(null);
   this.resetPhoto();
   if(item.hasPhoto)this.loadCurrentPhoto(item.id,id);
   this.personForm.reset(item.person);
   this.dispatchForm.reset(item.dispatch);
   // Başvuru grubu değişimini dinleyen form reseti muayene sebebini temizleyebilir.
   // Kayıt detayı yüklendiğinde seçili sebebi tekrar koru.
   this.dispatchForm.controls.reasonForExaminationDefId.setValue(item.dispatch.reasonForExaminationDefId,{emitEvent:false});
   this.identityType.set(item.person.tcKimlikNo?'tc':'foreign');
   this.dialogVisible.set(true);
   this.loadFiles(item.id);
   for(const field of [...this.personSelections,...this.dispatchSelections]){
    const selected=(this.personForm.get(field.key)?.value||this.dispatchForm.get(field.key)?.value) as string|null;
    if(selected)this.loadOptions(field.type,'',selected);
   }
  },error:e=>this.error(e)});
 }
 close(){if(this.saving())return;this.dialogVisible.set(false);this.qrVisible.set(false);this.resetPhoto();++this.editRequest;}
 /** Yalnız veritabanında kayıtlı kişi aranır; harici MERNİS/QR sorgusu simüle edilmez. */
 lookup(qr?:DispatchQrData){
  if(!this.canQuery())return;
  const f=this.personForm.getRawValue();
  if(!f.birthDate||!(this.identityType()==='tc'?f.tcKimlikNo:f.yabanciKimlikNo)){
   this.message.add({severity:'warn',summary:'Eksik bilgi',detail:'Kimlik numarası ve doğum tarihi giriniz.'});return;
  }
  this.queryingPerson.set(true);
  this.api.lookupPerson({tcKimlikNo:this.identityType()==='tc'?f.tcKimlikNo:null,
   yabanciKimlikNo:this.identityType()==='foreign'?f.yabanciKimlikNo:null,birthDate:f.birthDate})
   .pipe(finalize(()=>this.queryingPerson.set(false))).subscribe({
    next:r=>{this.personForm.patchValue(r.person);
     // QR ile aktarılan iletişim alanı, eski yerel kayıt dönüşünde kaybolmaz.
     if(qr?.telefonNo)this.personForm.controls.phoneNumber.setValue(qr.telefonNo);
     this.message.add({severity:'success',
     summary:'Yerel kişi bulundu',detail:'Kişi bilgileri dolduruldu.'});},
    error:e=>{const h=e as HttpErrorResponse;
     if(h.status===404)this.message.add({severity:'info',summary:'Kişi bulunamadı',
      detail:'Yerel kayıtta bulunamadı; kişi bilgilerini manuel doldurabilirsiniz.'});
     else this.error(e);}
   });
 }
 /** Dosya, kayıt başarılı olduktan sonra sevke bağlanır; başarısız upload ayrıca bildirilir. */
 selectFile(event:Event){
  const input=event.target as HTMLInputElement;
  const file=input.files?.[0]??null;
  input.value='';
  if(file&&(file.size>5242880||file.size<1||
    !['application/pdf','image/png','image/jpeg'].includes(file.type))){
    this.uploadFile.set(null);
    this.message.add({severity:'warn',summary:'Dosya seçilemedi',detail:'En fazla 5 MiB PDF, PNG veya JPEG dosyası seçiniz.'});
    return;
  }
  this.uploadFile.set(file);
 }
 save(){
  if(this.saving())return;
  this.formError.set(null);
  const person=this.personForm.getRawValue() as DispatchPerson;
  const dispatch=this.dispatchForm.getRawValue() as DispatchDetails;
  person.tcKimlikNo=this.identityType()==='tc'?person.tcKimlikNo?.trim()||null:null;
  person.yabanciKimlikNo=this.identityType()==='foreign'?person.yabanciKimlikNo?.trim()||null:null;
  if(this.form.invalid||!person.birthDate||!(person.tcKimlikNo||person.yabanciKimlikNo)){
   this.form.markAllAsTouched();this.formError.set('Ad, soyad, kimlik numarası, doğum tarihi, başvuru grubu ve muayene sebebi gereklidir.');return;
  }
  const rules=this.admissionRules();
  if(rules && (rules.nthAdmissionRequired&&!dispatch.nthAdmission ||
   rules.dispatchDocumentDateRequired&&!dispatch.dispatchDocumentDate ||
   rules.dispatchDocumentNoRequired&&!dispatch.dispatchDocumentNo?.trim() ||
   rules.forcesCommandRequired&&!dispatch.forcesCommandDefId ||
   rules.rankRequired&&!dispatch.rankDefId ||
   rules.militaryClassRequired&&!dispatch.militaryClassDefId ||
   rules.militaryUnitRequired&&!dispatch.militaryUnitDefId ||
   rules.militaryOfficeRequired&&!dispatch.militaryOfficeDefId ||
   rules.militarySchoolRequired&&!dispatch.militarySchoolDefId ||
   rules.registryNumberRequired&&!dispatch.registryNumber?.trim() ||
   rules.lastNominationDateRequired&&!dispatch.lastNominationDate ||
   rules.senderChairRequired&&!(dispatch.senderMilitaryUnitDefId||dispatch.senderMilitaryOfficeDefId))){
    this.formError.set('Başvuru grubunun zorunlu tuttuğu sevk ve askerî bilgileri doldurunuz.');return;
  }
  if(dispatch.dispatchScope===2&&!dispatch.dispatchScopeCityId ||
     dispatch.dispatchScope===3&&!dispatch.dispatchScopeHospitalId){
   this.formError.set('Sevk kapsamına uygun şehir veya sağlık tesisi seçiniz.');return;
  }
  const current=this.editing();
  this.saving.set(true);
  const task=current?this.api.update(current.id,current.version,person,dispatch):this.api.create(person,dispatch);
  task.subscribe({next:result=>this.onSaved(result),error:e=>{
   this.saving.set(false);this.formError.set(this.errorText(e));this.error(e);
  }});
 }
 private onSaved(result:DispatchSaveResponse){
  const file=this.uploadFile();
  if(file){
   this.api.uploadFile(result.id,file).pipe(finalize(()=>this.saving.set(false)))
    .subscribe({next:()=>this.finishSave(result.dispatchNo),error:e=>{
     this.dialogVisible.set(false);this.load();this.uploadFile.set(null);
     this.message.add({severity:'warn',summary:'Sevk kaydedildi',
      detail:'Sevk '+result.dispatchNo+' oluşturuldu ancak dosya yüklenemedi: '+this.errorText(e),life:8000});
    }});
  }else{this.saving.set(false);this.finishSave(result.dispatchNo);}
 }
 private finishSave(dispatchNo:number){
  this.dialogVisible.set(false);this.uploadFile.set(null);
  this.message.add({severity:'success',summary:'Sevk kaydedildi',
   detail:dispatchNo+' numaralı sevk başarıyla kaydedildi.'});
  this.load();
 }
 cancel(row:DispatchItem){
  if(!this.canUpdate()||row.currentState!==1)return;
  this.confirm.confirm({header:'Sevk İptali',message:row.dispatchNo+' numaralı sevk iptal edilsin mi? Kayıt geçmişte korunur.',
   acceptLabel:'İptal Et',rejectLabel:'Vazgeç',accept:()=>this.api.cancel(row).subscribe({
    next:()=>{this.message.add({severity:'success',summary:'Sevk iptal edildi'});this.load();},
    error:e=>this.error(e)})});
 }
 private loadFiles(id:string){
  this.api.files(id).subscribe({next:items=>this.files.set(items),error:e=>this.error(e)});
 }
 download(file:DispatchFileItem){
  const selected=this.editing();if(!selected)return;
  this.api.downloadFile(selected.id,file.id).subscribe({
   next:blob=>{const url=URL.createObjectURL(blob);const a=document.createElement('a');
    a.href=url;a.download=file.fileName;a.click();setTimeout(()=>URL.revokeObjectURL(url),1000);},
   error:e=>this.error(e)});
 }
 stateLabel(state:number):string{
  return ({1:'Yeni',2:'Provizyon',3:'Rapor',4:'İptal',5:'Bildirim'} as Record<number,string>)[state]??'Bilinmiyor';
 }
 stateSeverity(state:number):'success'|'warn'|'danger'|'info'{
  return state===4?'danger':state===1?'success':state===3?'info':'warn';
 }
 private errorText(e:unknown):string{
  const r=e as HttpErrorResponse;
  return r.error?.detail||r.error?.title||r.message||'İşlem gerçekleştirilemedi.';
 }
 private error(e:unknown){
  this.message.add({severity:'error',summary:'Sevk işlemi başarısız',detail:this.errorText(e),life:7500});
 }
}

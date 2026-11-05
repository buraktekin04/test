import {Component,computed,inject,signal} from '@angular/core';
import {RouterLink} from '@angular/router';
import {HttpErrorResponse} from '@angular/common/http';
import {ConfirmationService} from 'primeng/api';
import {ButtonModule} from 'primeng/button';
import {ConfirmDialogModule} from 'primeng/confirmdialog';
import {TableModule} from 'primeng/table';
import {TagModule} from 'primeng/tag';
import {DispatchService} from '../services/dispatch.service';
import type {BulkDispatchResponse} from '../models/dispatch-bulk.model';
import type {BulkDispatchItem,BulkParseProblem} from './dispatch-bulk.parser';
import {MAX_BULK_DISPATCH_FILE_BYTES,MAX_BULK_DISPATCH_ROWS,
 maskBulkIdentity,parseBulkDispatchFile} from './dispatch-bulk.parser';

/** Eski MSKBS Toplu Sevk Giriş: dosya, güvenli önizleme, satır bazlı sonuç. */
@Component({
 selector:'app-dispatch-bulk-entry',standalone:true,
 imports:[RouterLink,ButtonModule,ConfirmDialogModule,TableModule,TagModule],
 providers:[ConfirmationService],
 templateUrl:'./dispatch-bulk-entry.component.html',
 styleUrl:'./dispatch-bulk-entry.component.scss'
})
export class DispatchBulkEntryComponent {
 private readonly api=inject(DispatchService);
 private readonly confirmation=inject(ConfirmationService);
 readonly fileName=signal<string|null>(null);
 readonly fileLoading=signal(false);
 readonly error=signal<string|null>(null);
 readonly items=signal<BulkDispatchItem[]>([]);
 readonly problems=signal<BulkParseProblem[]>([]);
 readonly preview=signal('');
 readonly showPreview=signal(false);
 readonly saving=signal(false);
 readonly submitted=signal(false);
 readonly report=signal<BulkDispatchResponse|null>(null);
 readonly totalParsed=computed(()=>this.items().length+this.problems().length);
 readonly canSubmit=computed(()=>!!this.fileName()&&!this.fileLoading()&&!this.saving()&&
  !this.submitted()&&this.items().length>0&&this.problems().length===0);
 readonly maxRows=MAX_BULK_DISPATCH_ROWS;
 private fileVersion=0;

 async selectFile(event:Event):Promise<void>{
  const input=event.target as HTMLInputElement;
  const file=input.files?.[0];
  // Aynı dosya sonraki denemede de tekrar seçilebilsin.
  input.value='';
  ++this.fileVersion;
  this.items.set([]);
  this.problems.set([]);
  this.report.set(null);
  this.submitted.set(false);
  this.preview.set('');
  this.showPreview.set(false);
  this.error.set(null);
  this.fileName.set(file?.name??null);
  if(!file)return;
  if(!/\.(json|jsonl|txt|xml)$/i.test(file.name)){
   this.error.set('Yalnız JSON, JSONL, TXT veya içinde JSON bulunan eski XML uzantılı dosyalar yüklenebilir.');
   return;
  }
  if(file.size===0||file.size>MAX_BULK_DISPATCH_FILE_BYTES){
   this.error.set('Dosya boş olmamalı ve en fazla 1 MiB olabilir.');
   return;
  }
  const current=this.fileVersion;
  this.fileLoading.set(true);
  try{
   const raw=await file.text();
   if(current!==this.fileVersion)return;
   const parsed=parseBulkDispatchFile(raw);
   this.items.set(parsed.items);
   this.problems.set(parsed.problems);
   // Önizlemede yalnız beş kayıt bulunur, kimlik numaraları maskelenir.
   this.preview.set(JSON.stringify(parsed.items.slice(0,5).map(item=>({
    person:{...item.person,
     tcKimlikNo:maskBulkIdentity(item.person.tcKimlikNo),
     yabanciKimlikNo:maskBulkIdentity(item.person.yabanciKimlikNo)},
    dispatch:item.dispatch
   })),null,2));
   if(parsed.problems.length>0)this.error.set('Dosyada geçersiz satırlar bulunuyor. Düzeltilene kadar kayıt gönderilmeyecek.');
  }catch(e){
   if(current!==this.fileVersion)return;
   this.error.set(e instanceof Error?e.message:'Dosya okunamadı.');
  }finally{
   if(current===this.fileVersion)this.fileLoading.set(false);
  }
 }

 clear():void{
  if(this.saving())return;
  ++this.fileVersion;
  this.fileName.set(null);
  this.items.set([]);
  this.problems.set([]);
  this.report.set(null);
  this.submitted.set(false);
  this.showPreview.set(false);
  this.preview.set('');
  this.error.set(null);
  this.fileLoading.set(false);
 }

 confirmSave():void{
  if(!this.canSubmit())return;
  this.confirmation.confirm({
   header:'Toplu Sevk Kaydı',
   icon:'pi pi-exclamation-triangle',
   message:this.items().length+' sevk kaydedilecek. Başarılı kayıtlar kalıcıdır; başarısız satırlar diğer kayıtları geri almaz. Devam edilsin mi?',
   acceptLabel:'Sevkleri Kaydet',rejectLabel:'Vazgeç',
   accept:()=>this.save()
  });
 }

 private save():void{
  if(!this.canSubmit())return;
  const payload=this.items();
  this.saving.set(true);
  // Ağ kesintisinde belirsiz sonuçla tekrar POST atıp çift kayıt açmamak için kilitle.
  this.submitted.set(true);
  this.error.set(null);
  this.api.bulkCreate(payload).subscribe({
   next:response=>{
    this.report.set(response);
    this.saving.set(false);
    if(response.failureCount>0){
     this.error.set(response.successCount+' kayıt başarılı, '+response.failureCount+
       ' kayıt hatalı. Hatalı satırların mesajlarını kontrol ediniz.');
    }
   },
   error:(e:HttpErrorResponse)=>{
    this.saving.set(false);
    const detail=typeof e.error?.detail==='string'?e.error.detail:
     typeof e.error?.title==='string'?e.error.title:'Toplu sevk isteği tamamlanamadı.';
    this.error.set(detail+' Tekrar göndermeden önce Sevk Listesi’nde oluşan kayıtları kontrol ediniz.');
   }
  });
 }
}

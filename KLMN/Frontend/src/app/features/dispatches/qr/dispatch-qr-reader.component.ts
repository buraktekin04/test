import {ChangeDetectorRef,Component,ElementRef,EventEmitter,OnDestroy,Output,ViewChild,inject,signal} from '@angular/core';
import {BrowserQRCodeReader} from '@zxing/browser';
import type {IScannerControls} from '@zxing/browser';
import {ButtonModule} from 'primeng/button';
import {DialogModule} from 'primeng/dialog';

/**
 * Sevk QR okuyucu: USB/barkod klavyesi, metin, cihaz kamerası veya görsel dosyasını destekler.
 * Kamera akışı sadece kullanıcı eylemiyle başlar, kapanışta tüm track'ler durdurulur.
 */
@Component({
 selector:'app-dispatch-qr-reader',standalone:true,
 imports:[ButtonModule,DialogModule],
 templateUrl:'./dispatch-qr-reader.component.html',
 styleUrl:'./dispatch-qr-reader.component.scss'
})
export class DispatchQrReaderComponent implements OnDestroy {
 @Output() readonly scanned=new EventEmitter<string>();
 @Output() readonly dismissed=new EventEmitter<void>();
 @ViewChild('qrVideo') private qrVideo?:ElementRef<HTMLVideoElement>;
 private readonly cdr=inject(ChangeDetectorRef);
 private readonly reader=new BrowserQRCodeReader();
 private controls:IScannerControls|null=null;
 private token=0;
 readonly qrText=signal('');
 readonly cameraEnabled=signal(false);
 readonly cameraBusy=signal(false);
 readonly decodingImage=signal(false);
 readonly errorText=signal<string|null>(null);
 readonly infoText=signal('Kamerayı açabilir, QR görüntüsü yükleyebilir veya el okuyucusundan metin okutabilirsiniz.');

 /** Arka kamerayı tercih eder; yoksa tarayıcının uygun cihazını kullanır. */
 async startCamera(){
  if(this.cameraEnabled()||this.cameraBusy())return;
  const current=++this.token;
  this.errorText.set(null);this.cameraBusy.set(true);this.cameraEnabled.set(true);
  this.cdr.detectChanges();
  try{
   const element=this.qrVideo?.nativeElement;
   if(!element)throw new Error('Kamera görüntüsü oluşturulamadı.');
   const controls=await this.reader.decodeFromConstraints(
    {audio:false,video:{facingMode:{ideal:'environment'}}},
    element,
    (result)=>{
     if(result&&current===this.token){
      this.qrText.set(result.getText());
      this.infoText.set('QR okundu. Bilgileri forma aktarmak için aşağıdaki butonu kullanın.');
      this.stopCamera();
     }
    });
   if(current!==this.token){controls.stop();return;}
   this.controls=controls;
  }catch{
   if(current===this.token){
    this.errorText.set('Kamera açılamadı. HTTPS/localhost, kamera izni ve başka uygulamaların kamerayı kullanmadığını kontrol edin.');
    this.stopCamera();
   }
  }finally{
   this.cameraBusy.set(false);
  }
 }
 /** İzinli medya akışı ve okuyucuyu kapatır; önceden açılan cihazı boşta bırakmaz. */
 stopCamera(){
  ++this.token;
  this.controls?.stop();this.controls=null;
  const video=this.qrVideo?.nativeElement;
  if(video){
   const stream=video.srcObject;
   if(stream instanceof MediaStream)stream.getTracks().forEach(track=>track.stop());
   video.pause();video.srcObject=null;
  }
  this.cameraEnabled.set(false);
 }
 /** Telefonda kaydedilmiş QR görseli veya ekran görüntüsünden kod çözer. */
 async readImage(event:Event){
  const input=event.target as HTMLInputElement;
  const file=input.files?.[0];input.value='';
  if(!file)return;
  if(!['image/jpeg','image/png','image/webp'].includes(file.type)||file.size>5242880){
   this.errorText.set('QR görüntüsü JPEG, PNG veya WebP ve en fazla 5 MiB olmalıdır.');return;
  }
  this.stopCamera();
  this.errorText.set(null);
  this.decodingImage.set(true);
  const url=URL.createObjectURL(file);
  try{
   const decoded=await this.reader.decodeFromImageUrl(url);
   this.qrText.set(decoded.getText());
   this.infoText.set('Görüntüdeki QR kod çözüldü. Forma aktarabilirsiniz.');
  }catch{
   this.errorText.set('Seçilen görüntüde okunabilir bir QR kod bulunamadı.');
  }finally{
   URL.revokeObjectURL(url);
   this.decodingImage.set(false);
  }
 }
 updateText(event:Event){this.qrText.set((event.target as HTMLTextAreaElement).value);}
 /** Fiziksel okuyucular Enter gönderir; shift+Enter metin düzenlemek içindir. */
 onQrKeydown(event:KeyboardEvent){
  if(event.key==='Enter'&&!event.shiftKey){
   event.preventDefault();this.apply();
  }
 }
 apply(){
  if(!this.qrText().trim()){this.errorText.set('QR metni boş olamaz.');return;}
  this.errorText.set(null);
  this.scanned.emit(this.qrText().trim());
 }
 close(){
  this.stopCamera();
  this.dismissed.emit();
 }
 ngOnDestroy(){this.stopCamera();}
}

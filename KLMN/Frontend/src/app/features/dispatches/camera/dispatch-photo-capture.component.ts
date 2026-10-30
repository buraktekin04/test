import {ChangeDetectorRef,Component,ElementRef,EventEmitter,Input,OnDestroy,Output,ViewChild,inject,signal} from '@angular/core';
import {ButtonModule} from 'primeng/button';

/**
 * Eski Sevk Giriş "Kamera / Resim" akışı.
 * Fotoğraf cihaz üzerinde hazırlanır; yalnız kullanıcı formu kaydedince upload edilir.
 */
@Component({
 selector:'app-dispatch-photo-capture',standalone:true,
 imports:[ButtonModule],
 templateUrl:'./dispatch-photo-capture.component.html',
 styleUrl:'./dispatch-photo-capture.component.scss'
})
export class DispatchPhotoCaptureComponent implements OnDestroy {
 @Input() imageUrl:string|null=null;
 @Input() disabled=false;
 @Input() hasSavedPhoto=false;
 @Output() readonly photoSelected=new EventEmitter<File>();
 @Output() readonly photoCleared=new EventEmitter<void>();
 @Output() readonly deleteSavedPhoto=new EventEmitter<void>();
 @ViewChild('photoVideo') private photoVideo?:ElementRef<HTMLVideoElement>;
 private readonly cdr=inject(ChangeDetectorRef);
 private stream:MediaStream|null=null;
 private generation=0;
 readonly cameraOpen=signal(false);
 readonly cameraBusy=signal(false);
 readonly photoError=signal<string|null>(null);
 readonly capturing=signal(false);

 /** Kamera izni yalnız tıklamayla istenir; ön kamera kişi fotoğrafı için tercih edilir. */
 async openCamera(){
  if(this.disabled||this.cameraOpen()||this.cameraBusy())return;
  const generation=++this.generation;
  this.photoError.set(null);
  this.cameraBusy.set(true);
  this.cameraOpen.set(true);
  this.cdr.detectChanges();
  try{
   if(!navigator.mediaDevices?.getUserMedia)throw new Error('Bu tarayıcıda kamera kullanılamıyor.');
   const stream=await navigator.mediaDevices.getUserMedia({
    audio:false,video:{facingMode:{ideal:'user'},width:{ideal:1280},height:{ideal:720}}
   });
   if(generation!==this.generation){stream.getTracks().forEach(track=>track.stop());return;}
   this.stream=stream;
   const video=this.photoVideo?.nativeElement;
   if(!video)throw new Error('Kamera önizlemesi bulunamadı.');
   video.srcObject=stream;
   video.muted=true;
   video.playsInline=true;
   await video.play();
  }catch{
   if(generation===this.generation){
    this.photoError.set('Kamera açılamadı. HTTPS/localhost, kamera izinlerini ve cihaz bağlantısını kontrol edin.');
    this.closeCamera();
   }
  }finally{
   this.cameraBusy.set(false);
  }
 }
 /** Video stream'inin tüm track'lerini kapatır; sekme/ekran kapatılırken kamera açık kalmaz. */
 closeCamera(){
  ++this.generation;
  this.stream?.getTracks().forEach(track=>track.stop());
  this.stream=null;
  const video=this.photoVideo?.nativeElement;
  if(video){video.pause();video.srcObject=null;}
  this.cameraOpen.set(false);
 }
 /** JPEG önizlemesi için resmi ölçeklendirir ve 2 MiB sınırına uyum sağlar. */
 async capture(){
  const video=this.photoVideo?.nativeElement;
  if(!video||!video.videoWidth||!video.videoHeight||this.capturing())return;
  this.capturing.set(true);this.photoError.set(null);
  try{
   const factor=Math.min(1,1280/video.videoWidth);
   const canvas=document.createElement('canvas');
   canvas.width=Math.max(1,Math.round(video.videoWidth*factor));
   canvas.height=Math.max(1,Math.round(video.videoHeight*factor));
   const context=canvas.getContext('2d');
   if(!context)throw new Error('Fotoğraf oluşturulamadı.');
   context.drawImage(video,0,0,canvas.width,canvas.height);
   const blob=await new Promise<Blob|null>(resolve=>canvas.toBlob(resolve,'image/jpeg',.84));
   if(!blob||blob.size>2097152)throw new Error('Kamera fotoğrafı 2 MiB sınırını aşıyor.');
   const file=new File([blob],'sevk-fotograf-'+Date.now()+'.jpg',{type:'image/jpeg'});
   this.photoSelected.emit(file);
   this.closeCamera();
  }catch{
   this.photoError.set('Fotoğraf çekilemedi. Kamerayı tekrar deneyin veya resim yükleyin.');
  }finally{
   this.capturing.set(false);
  }
 }
 /** Bilgisayardan/telefondan JPEG-PNG seçimi. Tür ve boyut backend'de ayrıca kontrol edilir. */
 chooseImage(event:Event){
  const input=event.target as HTMLInputElement;
  const file=input.files?.[0];input.value='';
  if(!file)return;
  if(!['image/jpeg','image/png'].includes(file.type)||file.size<1||file.size>2097152){
   this.photoError.set('Fotoğraf JPEG/PNG ve en fazla 2 MiB olmalıdır.');return;
  }
  this.photoError.set(null);
  this.closeCamera();
  this.photoSelected.emit(file);
 }
 clearImage(){
  this.closeCamera();
  this.photoCleared.emit();
 }
 removeExisting(){
  this.closeCamera();
  this.deleteSavedPhoto.emit();
 }
 ngOnDestroy(){this.closeCamera();}
}

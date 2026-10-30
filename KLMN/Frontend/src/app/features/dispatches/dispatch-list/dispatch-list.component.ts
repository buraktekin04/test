import {DatePipe} from '@angular/common';
import {HttpErrorResponse} from '@angular/common/http';
import {Component,computed,inject,OnInit,signal} from '@angular/core';
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
import {DispatchItem} from '../models/dispatch.model';
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
export class DispatchListComponent implements OnInit {
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
 readonly auditRecord=signal<DispatchItem|null>(null);
 private lastRequest=0;

 readonly statusItems=[
  {label:'Tüm durumlar',value:null},
  {label:'Yeni',value:1},
  {label:'Provizyon',value:2},
  {label:'Rapor',value:3},
  {label:'İptal',value:4},
  {label:'Bildirim',value:5}
 ];
 readonly searchForm=this.fb.group({
  search:this.fb.nonNullable.control(''),
  includeCancelled:this.fb.nonNullable.control(false),
  currentState:this.fb.control<number|null>(null),
  documentDateFrom:this.fb.nonNullable.control(''),
  documentDateTo:this.fb.nonNullable.control('')
 });
 readonly filterError=signal<string|null>(null);

 ngOnInit(){
  const saved=this.route.snapshot.queryParamMap.get('saved');
  if(saved && /^\d+$/.test(saved)){
   const hasFailures=this.route.snapshot.queryParamMap.get('attachmentsFailed');
   this.message.add({
    severity:hasFailures&&Number(hasFailures)>0?'warn':'success',
    summary:hasFailures&&Number(hasFailures)>0?'Sevk kaydedildi; ekleri kontrol edin':'Sevk başarıyla kaydedildi',
    detail:saved+' numaralı sevkin kayıt işlemi tamamlandı.'+
     (hasFailures&&Number(hasFailures)>0?' Bazı dosyalar yüklenememiş olabilir.':''),
    life:7500
   });
  }
  if(this.canView())this.load();
 }

 /** Yalnız backend üzerinde filtrelenmiş sayfayı çağırır; sayfalar arası sonuç kaybetmez. */
 load(){
  if(!this.canView())return;
  const request=++this.lastRequest;
  this.loading.set(true);
  const form=this.searchForm.getRawValue();
  this.api.list(this.page(),this.pageSize(),form.search,form.includeCancelled,
    form.currentState,form.documentDateFrom,form.documentDateTo)
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
  if(values.documentDateFrom&&values.documentDateTo&&values.documentDateFrom>values.documentDateTo){
   this.filterError.set('Başlangıç tarihi bitiş tarihinden sonra olamaz.');
   return;
  }
  this.filterError.set(null);
  this.page.set(1);
  this.load();
 }
 resetSearch(){
  this.searchForm.reset({search:'',includeCancelled:false,currentState:null,documentDateFrom:'',documentDateTo:''});
  this.search();
 }
 toPage(n:number){if(n<1||n>this.maxPage())return;this.page.set(n);this.load();}
 setPageSize(e:Event){
  const value=Number((e.target as HTMLSelectElement).value);
  if(![10,20,50,100].includes(value))return;
  this.pageSize.set(value);this.search();
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
  return ({1:'Yeni',2:'Provizyon',3:'Rapor',4:'İptal',5:'Bildirim'} as Record<number,string>)[state]??'Bilinmiyor';
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

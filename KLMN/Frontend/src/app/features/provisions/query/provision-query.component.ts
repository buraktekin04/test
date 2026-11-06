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
import {DispatchService} from '../../dispatches/services/dispatch.service';
import type {DispatchOption} from '../../dispatches/models/dispatch.model';
import {ProvisionService} from '../services/provision.service';
import type {ProvisionItem,ProvisionDetail,ProvisionFilter,ProvisionStatusOption} from '../models/provision.model';

@Component({
 selector:'app-provision-query',standalone:true,
 imports:[ReactiveFormsModule,FormsModule,RouterLink,DatePipe,ButtonModule,TableModule,
  SelectModule,DialogModule,TagModule],
 templateUrl:'./provision-query.component.html',
 styleUrl:'../provision-common.scss'
})
export class ProvisionQueryComponent implements OnInit {
 private readonly api=inject(ProvisionService);
 private readonly dispatchApi=inject(DispatchService);
 private readonly route=inject(ActivatedRoute);
 private readonly fb=inject(FormBuilder);
 private readonly permissions=inject(PermissionService);
 readonly canCancel=computed(()=>this.permissions.has(PermissionCodes.Provisions.Cancel));
 readonly hospitals=signal<DispatchOption[]>([]);
 readonly statuses=signal<ProvisionStatusOption[]>([]);
 readonly detailHospitalName=signal('');
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
 cancelReason='';
 readonly form=this.fb.nonNullable.group({
  dateFrom:'',dateTo:'',currentState:0,provisionNo:0,hospitalId:'',
  tcKimlikNo:'',yabanciKimlikNo:'',name:'',surname:'',purchaseId:0
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
   purchaseId:v.purchaseId||undefined
  };
 }
 search(newPage=1):void{
  if(this.loading())return;
  const v=this.form.getRawValue();
  if(v.dateFrom&&v.dateTo&&v.dateFrom>v.dateTo){
   this.error.set('Bitiş tarihi başlangıç tarihinden önce olamaz.');return;
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
 openDetail(row:ProvisionItem):void{
  this.detail.set(null);this.detailHospitalName.set(row.hospitalName);this.detailOpen.set(true);
  this.api.detail(row.id).subscribe({
   next:data=>this.detail.set(data),
   error:e=>{this.error.set(e.error?.detail||'Provizyon detayı alınamadı.');this.detailOpen.set(false);}
  });
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

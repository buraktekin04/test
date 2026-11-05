import {Component,inject,OnInit,signal} from '@angular/core';
import {DatePipe} from '@angular/common';
import {ReactiveFormsModule,FormBuilder} from '@angular/forms';
import {RouterLink,ActivatedRoute} from '@angular/router';
import {ButtonModule} from 'primeng/button';
import {TableModule} from 'primeng/table';
import {TagModule} from 'primeng/tag';
import {ProvisionService} from '../services/provision.service';
import type {ProvisionItem,ProvisionState} from '../models/provision.model';

@Component({
 selector:'app-provision-tracking',standalone:true,
 imports:[ReactiveFormsModule,RouterLink,DatePipe,ButtonModule,TableModule,TagModule],
 templateUrl:'./provision-tracking.component.html',
 styleUrl:'../provision-common.scss'
})
export class ProvisionTrackingComponent implements OnInit {
 private readonly api=inject(ProvisionService);
 private readonly fb=inject(FormBuilder);
 private readonly route=inject(ActivatedRoute);
 readonly tab=signal<'detail'|'summary'>('detail');
 readonly loading=signal(false);
 readonly error=signal('');
 readonly records=signal<ProvisionItem[]>([]);
 readonly totalCount=signal(0);
 readonly page=signal(1);
 readonly selected=signal<ProvisionItem|null>(null);
 readonly states=signal<ProvisionState[]>([]);
 readonly searched=signal(false);
 readonly form=this.fb.nonNullable.group({
  tcKimlikNo:'',purchaseId:0,provisionNo:0,dateFrom:'',dateTo:''
 });
 ngOnInit():void{
  const query=this.route.snapshot.queryParamMap.get('provisionNo');
  if(query&&/^\d+$/.test(query)){this.form.patchValue({provisionNo:Number(query)});this.search();}
 }
 stateLabel(state:number):string{
  return ({1:'Yeni',2:'Rapor',3:'İptal',4:'Bildirim'} as Record<number,string>)[state]??'Bilinmiyor';
 }
 search(nextPage=1):void{
  if(this.loading())return;
  const v=this.form.getRawValue();
  if(v.dateFrom&&v.dateTo&&v.dateFrom>v.dateTo){
   this.error.set('Bitiş tarihi başlangıç tarihinden önce olamaz.');return;
  }
  if(this.tab()==='detail'&&!v.tcKimlikNo&&!v.purchaseId&&!v.provisionNo){
   this.error.set('Detay aramasında T.C. Kimlik No, Alım ID veya Provizyon No giriniz.');return;
  }
  this.error.set('');this.loading.set(true);this.page.set(nextPage);
  this.selected.set(null);this.states.set([]);
  this.api.list({
   tcKimlikNo:v.tcKimlikNo.trim()||undefined,
   purchaseId:v.purchaseId||undefined,
   provisionNo:v.provisionNo||undefined,
   dateFrom:v.dateFrom||undefined,dateTo:v.dateTo||undefined
  },nextPage,20).subscribe({
   next:r=>{this.records.set(r.items);this.totalCount.set(r.totalCount);
    this.loading.set(false);this.searched.set(true);},
   error:e=>{this.loading.set(false);this.error.set(e.error?.detail||'Provizyon takibi sorgulanamadı.');}
  });
 }
 select(row:ProvisionItem):void{
  this.selected.set(row);this.states.set([]);this.error.set('');
  this.api.states(row.id).subscribe({
   next:states=>this.states.set(states),
   error:e=>this.error.set(e.error?.detail||'Durum geçmişi alınamadı.')
  });
 }
 setTab(value:'detail'|'summary'):void{
  this.tab.set(value);this.records.set([]);this.selected.set(null);this.states.set([]);
  this.searched.set(false);this.totalCount.set(0);this.error.set('');
 }
}

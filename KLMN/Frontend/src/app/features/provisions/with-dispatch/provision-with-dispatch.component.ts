import {Component,inject,signal,OnInit} from '@angular/core';
import {ReactiveFormsModule,FormBuilder,Validators} from '@angular/forms';
import {ActivatedRoute,RouterLink} from '@angular/router';
import {ButtonModule} from 'primeng/button';
import {SelectModule} from 'primeng/select';
import {DispatchService} from '../../dispatches/services/dispatch.service';
import type {DispatchOption} from '../../dispatches/models/dispatch.model';
import {ProvisionService} from '../services/provision.service';
import type {ProvisionMutation} from '../models/provision.model';

@Component({
 selector:'app-provision-with-dispatch',standalone:true,
 imports:[ReactiveFormsModule,RouterLink,ButtonModule,SelectModule],
 templateUrl:'./provision-with-dispatch.component.html',
 styleUrl:'./provision-with-dispatch.component.scss'
})
export class ProvisionWithDispatchComponent implements OnInit {
 private readonly fb=inject(FormBuilder);
 private readonly route=inject(ActivatedRoute);
 private readonly api=inject(ProvisionService);
 private readonly dispatchApi=inject(DispatchService);
 readonly hospitals=signal<DispatchOption[]>([]);
 readonly loading=signal(false);
 readonly error=signal('');
 readonly result=signal<ProvisionMutation|null>(null);
 readonly form=this.fb.nonNullable.group({
  dispatchReference:['',Validators.required],
  hospitalId:['',Validators.required]
 });
 ngOnInit():void{
  const id=this.route.snapshot.queryParamMap.get('dispatchId')||
   this.route.snapshot.paramMap.get('dispatchId');
  if(id)this.form.controls.dispatchReference.setValue(id);
  this.dispatchApi.options('hospitals').subscribe({
   next:options=>this.hospitals.set(options),
   error:()=>this.error.set('Hastane listesi yüklenemedi.')
  });
 }
 save():void{
  if(this.form.invalid||this.loading())return;
  this.error.set('');this.result.set(null);
  this.loading.set(true);
  const v=this.form.getRawValue();
  try{
   this.api.withDispatch(v.dispatchReference,v.hospitalId).subscribe({
    next:result=>{this.result.set(result);this.loading.set(false);},
    error:e=>{this.error.set(e.error?.detail||e.error?.title||'Provizyon alınamadı.');this.loading.set(false);}
   });
  }catch(e){this.error.set(e instanceof Error?e.message:'Geçersiz sevk.');this.loading.set(false);}
 }
}

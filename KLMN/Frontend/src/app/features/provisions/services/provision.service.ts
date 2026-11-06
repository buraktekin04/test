import {inject,Injectable} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {API_BASE_URL} from '../../../core/config/api.tokens';
import type {DispatchDetails,DispatchPerson} from '../../dispatches/models/dispatch.model';
import type {ProvisionDetail,ProvisionFilter,ProvisionItem,ProvisionMutation,ProvisionPage,ProvisionState,ProvisionStatusOption} from '../models/provision.model';

/** API uçları kullanıcı yetkisini ayrıca sunucu üzerinde kontrol eder. */
@Injectable({providedIn:'root'})
export class ProvisionService {
 private readonly http=inject(HttpClient);
 private readonly url=inject(API_BASE_URL)+'/provisions';

 withDispatch(dispatchReference:string,hospitalId:string){
  const dispatchId=/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(dispatchReference.trim())
   ? dispatchReference.trim():null;
  const dispatchNo=!dispatchId && /^[1-9]\d*$/.test(dispatchReference.trim())
   ? Number(dispatchReference.trim()):null;
  if(!dispatchId&&(!dispatchNo||!Number.isSafeInteger(dispatchNo)))
   throw new Error('Geçerli bir sevk numarası giriniz.');
  return this.http.post<ProvisionMutation>(this.url+'/with-dispatch',{dispatchId,dispatchNo,hospitalId});
 }
 withoutDispatch(hospitalId:string,person:DispatchPerson,dispatch:DispatchDetails,
  objectionableReportId:string|null=null,photoBase64:string|null=null){
  return this.http.post<ProvisionMutation>(this.url+'/without-dispatch',
   {hospitalId,person,dispatch,objectionableReportId,photoBase64});
 }
 list(filters:ProvisionFilter={},pageNumber=1,pageSize=20){
  let params=new HttpParams().set('pageNumber',pageNumber).set('pageSize',pageSize);
  for(const [key,value] of Object.entries(filters)){
   if(value!==undefined&&value!==null&&value!=='')params=params.set(key,String(value));
  }
  return this.http.get<ProvisionPage>(this.url,{params});
 }
 statuses(){return this.http.get<ProvisionStatusOption[]>(this.url+'/statuses');}
 detail(id:string){return this.http.get<ProvisionDetail>(this.url+'/'+id);}
 states(id:string){return this.http.get<ProvisionState[]>(this.url+'/'+id+'/states');}
 cancel(item:Pick<ProvisionItem,'id'|'version'>,reason:string){
  return this.http.patch<ProvisionMutation>(this.url+'/'+item.id+'/cancel',
   {id:item.id,version:item.version,reason});
 }
}

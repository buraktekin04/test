import {Injectable,inject} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {API_BASE_URL} from '../../../core/config/api.tokens';
import {SyyRequiredItem,SyyRequiredOption,SyyRequiredQuery,PageResult} from '../models/syy-diagnosis-required.model';
/** SYY Onay Gerektiren Maddeler için tek ekran/iki tabloyu tek CRUD sözleşmesinde birleştirir. */
@Injectable({providedIn:'root'})
export class SyyDiagnosisRequiredService {
 private readonly http=inject(HttpClient);
 private readonly url=inject(API_BASE_URL)+'/syy-diagnosis-requireds';
 private params(q:SyyRequiredQuery){
  let p=new HttpParams().set('pageNumber',q.pageNumber).set('pageSize',q.pageSize)
   .set('includeInactive',q.includeInactive);
  if(q.search.trim())p=p.set('search',q.search.trim());
  if(q.isActive!==null)p=p.set('isActive',q.isActive);
  if(q.syyDiagnosisDefId)p=p.set('syyDiagnosisDefId',q.syyDiagnosisDefId);
  return p;
 }
 list(q:SyyRequiredQuery){return this.http.get<PageResult<SyyRequiredItem>>(this.url,{params:this.params(q)});}
 detail(id:string){return this.http.get<SyyRequiredItem>(this.url+'/'+id);}
 options(selectedId?:string){let p=new HttpParams();if(selectedId)p=p.set('selectedDiagnosisId',selectedId);
   return this.http.get<SyyRequiredOption[]>(this.url+'/options',{params:p});}
 create(data:Record<string,unknown>){return this.http.post<{id:string;version:number}>(this.url,data);}
 update(id:string,data:Record<string,unknown>){return this.http.put<{id:string;version:number}>(this.url+'/'+id,data);}
 status(x:SyyRequiredItem){return this.http.patch(this.url+'/'+x.id+'/status',
  {id:x.id,version:x.version,isActive:!x.isActive});}
 delete(x:SyyRequiredItem){return this.http.delete(this.url+'/'+x.id,
  {params:new HttpParams().set('version',x.version)});}
 export(q:SyyRequiredQuery){return this.http.get(this.url+'/export',{params:this.params(q),responseType:'blob'});}
}

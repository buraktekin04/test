import {inject,Injectable} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {API_BASE_URL} from '../../../core/config/api.tokens';
import {PagedResult} from '../../../core/models/paged-result.model';
import {HcDecisionItem,HcDecisionQuery,HcDecisionFields,HcDecisionMutationResponse} from '../models/hc-decision.model';
/** Filtreleme/sayfalama sunucudadır; tablo kolon sıralaması PrimeNG'dedir. */
function params(q:HcDecisionQuery):HttpParams {
 let p=new HttpParams().set('search',q.search.trim()).set('includeInactive',q.includeInactive)
   .set('pageNumber',q.pageNumber).set('pageSize',q.pageSize);
 if(q.isActive!==null)p=p.set('isActive',q.isActive);
 return p;
}
/** Kurul Karar Tanımları HTTP CRUD sözleşmesi. */
@Injectable({providedIn:'root'})
export class HcDecisionsService {
 private http=inject(HttpClient);private url=inject(API_BASE_URL)+'/hc-decisions';
 list(q:HcDecisionQuery){return this.http.get<PagedResult<HcDecisionItem>>(this.url,{params:params(q)});}
 getById(id:string){return this.http.get<HcDecisionItem>(this.url+'/'+id);}
 create(v:HcDecisionFields){return this.http.post<HcDecisionMutationResponse>(this.url,v);}
 update(v:HcDecisionFields&{id:string;version:number}){return this.http.put<HcDecisionMutationResponse>(this.url+'/'+v.id,v);}
 changeStatus(id:string,version:number,isActive:boolean){return this.http.patch<HcDecisionMutationResponse>(this.url+'/'+id+'/status',{id,version,isActive});}
 delete(id:string,version:number){return this.http.delete<void>(this.url+'/'+id,{params:new HttpParams().set('version',version)});}
 export(q:HcDecisionQuery){return this.http.get(this.url+'/export',{params:params(q),responseType:'blob'});}
}

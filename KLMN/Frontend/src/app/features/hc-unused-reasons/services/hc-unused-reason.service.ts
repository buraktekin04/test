import {inject,Injectable} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {API_BASE_URL} from '../../../core/config/api.tokens';
import {PagedResult} from '../../../core/models/paged-result.model';
import {HcDecisionRelationItem,HcDecisionRelationFields,HcDecisionRelationOptions,HcDecisionRelationQuery,HcDecisionMutationResponse} from '../models/hc-unused-reason.model';
/** Kullanılmayacak Muayene Sebepleri için ayrı HTTP servisi; filtreleme sunucuda yapılır. */
@Injectable({providedIn:'root'})
export class HcUnusedReasonsService {
 private http=inject(HttpClient);private url=inject(API_BASE_URL)+'/hc-unused-reasons';
 private params(q:HcDecisionRelationQuery):HttpParams {
  let p=new HttpParams().set('search',q.search.trim()).set('includeInactive',q.includeInactive).set('pageNumber',q.pageNumber).set('pageSize',q.pageSize);
  if(q.isActive!==null)p=p.set('isActive',q.isActive);
  if(q.hcDecisionDefId)p=p.set('hcDecisionDefId',q.hcDecisionDefId);
  return p;
 }
 list(q:HcDecisionRelationQuery){return this.http.get<PagedResult<HcDecisionRelationItem>>(this.url,{params:this.params(q)});}
 getById(id:string){return this.http.get<HcDecisionRelationItem>(this.url+'/'+id);}
 options(decisionId?:string|null,refId?:string|null){
  let p=new HttpParams();
  if(decisionId)p=p.set('selectedDecisionId',decisionId);
  if(refId)p=p.set('selectedReferenceId',refId);
  return this.http.get<HcDecisionRelationOptions>(this.url+'/options',{params:p});
 }
 create(v:HcDecisionRelationFields){return this.http.post<HcDecisionMutationResponse>(this.url,v);}
 update(v:HcDecisionRelationFields&{id:string;version:number}){return this.http.put<HcDecisionMutationResponse>(this.url+'/'+v.id,v);}
 changeStatus(id:string,version:number,isActive:boolean){return this.http.patch<HcDecisionMutationResponse>(this.url+'/'+id+'/status',{id,version,isActive});}
 delete(id:string,version:number){return this.http.delete<void>(this.url+'/'+id,{params:new HttpParams().set('version',version)});}
 export(q:HcDecisionRelationQuery){return this.http.get(this.url+'/export',{params:this.params(q),responseType:'blob'});}
}

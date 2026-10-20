import {inject,Injectable} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';
import {API_BASE_URL} from '../../../core/config/api.tokens';
import {PagedResult} from '../../../core/models/paged-result.model';
import {ReasonItem,ReasonQuery,ReasonFields,MutationResponse,RelationOptions,RelationQuery,RelationFields,RelationItem} from '../models/reason.model';
/** Ortak HTTP parametreleri; kolon sıralaması yalnız önyüzde yapılır. */
function params(q:ReasonQuery):HttpParams {
 let p=new HttpParams().set('search',q.search.trim()).set('includeInactive',q.includeInactive)
 .set('pageNumber',q.pageNumber).set('pageSize',q.pageSize);
 if(q.isActive!==null)p=p.set('isActive',q.isActive);
 if('reasonForExaminationDefId' in q && q.reasonForExaminationDefId)
 p=p.set('reasonForExaminationDefId',String(q.reasonForExaminationDefId));
 return p;
}
/** Muayene sebebi ana CRUD servisi. */
@Injectable({providedIn:'root'})
export class ReasonsService {
 private http=inject(HttpClient);private url=inject(API_BASE_URL)+'/reasons-for-examination';
 list(q:ReasonQuery):Observable<PagedResult<ReasonItem>>{return this.http.get<PagedResult<ReasonItem>>(this.url,{params:params(q)});}
 getById(id:string){return this.http.get<ReasonItem>(this.url+'/'+id);}
 create(value:ReasonFields){return this.http.post<MutationResponse>(this.url,value);}
 update(value:ReasonFields&{id:string;version:number}){return this.http.put<MutationResponse>(this.url+'/'+value.id,value);}
 changeStatus(id:string,version:number,isActive:boolean){return this.http.patch<MutationResponse>(this.url+'/'+id+'/status',{id,version,isActive});}
 delete(id:string,version:number){return this.http.delete<void>(this.url+'/'+id,{params:new HttpParams().set('version',version)});}
 export(q:ReasonQuery){return this.http.get(this.url+'/export',{params:params(q),responseType:'blob'});}
}
/** Dört alt ekranın her biri kendi route ve permission kodunu kullanır. */
@Injectable({providedIn:'root'})
export class ReasonRelationsService {
 private http=inject(HttpClient);private api=inject(API_BASE_URL);
 private path(slug:string){return this.api+'/'+slug;}
 list(slug:string,q:RelationQuery){return this.http.get<PagedResult<RelationItem>>(this.path(slug),{params:params(q)});}
 getById(slug:string,id:string){return this.http.get<RelationItem>(this.path(slug)+'/'+id);}
 options(slug:string,reasonId?:string|null,referenceId?:string|null){
  let p=new HttpParams();
  if(reasonId)p=p.set('selectedReasonId',reasonId);
  if(referenceId)p=p.set('selectedReferenceId',referenceId);
  return this.http.get<RelationOptions>(this.path(slug)+'/options',{params:p});
 }
 create(slug:string,value:RelationFields){return this.http.post<MutationResponse>(this.path(slug),value);}
 update(slug:string,value:RelationFields&{id:string;version:number}){return this.http.put<MutationResponse>(this.path(slug)+'/'+value.id,value);}
 changeStatus(slug:string,id:string,version:number,isActive:boolean){
 return this.http.patch<MutationResponse>(this.path(slug)+'/'+id+'/status',{id,version,isActive});}
 delete(slug:string,id:string,version:number){
 return this.http.delete<void>(this.path(slug)+'/'+id,{params:new HttpParams().set('version',version)});}
 export(slug:string,q:RelationQuery){return this.http.get(this.path(slug)+'/export',{params:params(q),responseType:'blob'});}
}

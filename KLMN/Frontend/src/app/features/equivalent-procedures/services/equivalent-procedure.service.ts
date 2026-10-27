import {inject,Injectable} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {API_BASE_URL} from '../../../core/config/api.tokens';
import {EquivalentProcedureItem,EquivalentProcedureOption,EquivalentProcedureFields,
 EquivalentProcedureMutationResponse,EquivalentProcedurePagedResult,
 EquivalentProcedureQuery} from '../models/equivalent-procedure.model';
/** Eşdeğer tetkik CRUD ve server-side arama seçenekleri. */
@Injectable({providedIn:'root'})
export class EquivalentProcedureService {
 private readonly http=inject(HttpClient);
 private readonly url=inject(API_BASE_URL)+'/equivalent-procedures';
 private params(q:EquivalentProcedureQuery):HttpParams {
  let p=new HttpParams().set('pageNumber',q.pageNumber).set('pageSize',q.pageSize)
   .set('includeInactive',q.includeInactive);
  if(q.search.trim())p=p.set('search',q.search.trim());
  if(q.procedureId)p=p.set('procedureId',q.procedureId);
  if(q.isActive!==null)p=p.set('isActive',q.isActive);
  return p;
 }
 list(q:EquivalentProcedureQuery){
  return this.http.get<EquivalentProcedurePagedResult>(this.url,{params:this.params(q)});
 }
 /** Açılır öneri paneline en fazla 60 eşleşen tetkik yükler. */
 options(search:string,selectedId?:string){
  let p=new HttpParams().set('search',search);
  if(selectedId)p=p.set('selectedId',selectedId);
  return this.http.get<EquivalentProcedureOption[]>(this.url+'/options',{params:p});
 }
 detail(id:string){return this.http.get<EquivalentProcedureItem>(this.url+'/'+id);}
 create(r:EquivalentProcedureFields){
  return this.http.post<EquivalentProcedureMutationResponse>(this.url,r);
 }
 update(r:EquivalentProcedureFields&{id:string;version:number}){
  return this.http.put<EquivalentProcedureMutationResponse>(this.url+'/'+r.id,r);
 }
 status(item:EquivalentProcedureItem){
  return this.http.patch<EquivalentProcedureMutationResponse>(this.url+'/'+item.id+'/status',
   {id:item.id,version:item.version,isActive:!item.isActive});
 }
 delete(item:EquivalentProcedureItem){
  return this.http.delete<void>(this.url+'/'+item.id,{params:new HttpParams().set('version',item.version)});
 }
 export(q:EquivalentProcedureQuery){
  return this.http.get(this.url+'/export',{params:this.params(q),responseType:'blob'});
 }
}

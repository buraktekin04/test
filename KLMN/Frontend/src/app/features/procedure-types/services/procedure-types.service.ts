import {inject,Injectable} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {API_BASE_URL} from '../../../core/config/api.tokens';
import {PagedResult} from '../../../core/models/paged-result.model';
import {ProcedureTypeItem,ProcedureTypeMutation,ProcedureTypeQuery} from '../models/procedure-types.model';
/** Tetkik Türü Tanımları CRUD ve Excel API istemcisi. */
@Injectable({providedIn:'root'})
export class ProcedureTypeService {
 private readonly http=inject(HttpClient);
 private readonly url=inject(API_BASE_URL)+'/procedure-types';
 private params(q:ProcedureTypeQuery) {
   let p=new HttpParams().set('pageNumber',q.pageNumber).set('pageSize',q.pageSize)
   .set('includeInactive',q.includeInactive);
   if(q.search.trim())p=p.set('search',q.search.trim());
   if(q.isActive!==null)p=p.set('isActive',q.isActive);
   return p;
 }
 list(q:ProcedureTypeQuery){return this.http.get<PagedResult<ProcedureTypeItem>>(this.url,{params:this.params(q)});}
 getById(id:string){return this.http.get<ProcedureTypeItem>(this.url+'/'+id);}
 create(code:number,name:string){return this.http.post<ProcedureTypeMutation>(this.url,{code,name});}
 update(id:string,version:number,name:string){return this.http.put<ProcedureTypeMutation>(this.url+'/'+id,{id,version,name});}
 status(id:string,version:number,isActive:boolean){
   return this.http.patch<ProcedureTypeMutation>(this.url+'/'+id+'/status',{id,version,isActive});
 }
 delete(id:string,version:number){return this.http.delete<void>(this.url+'/'+id,{params:new HttpParams().set('version',version)});}
 export(q:ProcedureTypeQuery){return this.http.get(this.url+'/export',{params:this.params(q),responseType:'blob'});}
}

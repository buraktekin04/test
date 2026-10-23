import {inject,Injectable} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {API_BASE_URL} from '../../../core/config/api.tokens';
import {Icd10DefItem,DefinitionQuery,PageResult} from '../models/icd10-def.model';
/** ICD10Defs modülünün bağımsız API servisi; ekran adresi kodda sabittir. */
@Injectable({providedIn:'root'})
export class Icd10DefService {
 private readonly http=inject(HttpClient);
 private readonly url=inject(API_BASE_URL)+'/icd10-defs';
 private params(q:DefinitionQuery):HttpParams {
  let p=new HttpParams().set('pageNumber',q.pageNumber).set('pageSize',q.pageSize)
   .set('includeInactive',q.includeInactive);
  if(q.search.trim())p=p.set('search',q.search.trim());
  if(q.isActive!==null)p=p.set('isActive',q.isActive);
  return p;
 }
 list(q:DefinitionQuery){return this.http.get<PageResult<Icd10DefItem>>(this.url,{params:this.params(q)});}
 detail(id:string){return this.http.get<Icd10DefItem>(this.url+'/'+id);}
 create(payload:Record<string,unknown>){return this.http.post<{id:string;version:number}>(this.url,payload);}
 update(id:string,payload:Record<string,unknown>){return this.http.put<{id:string;version:number}>(this.url+'/'+id,payload);}
 status(id:string,version:number,isActive:boolean){
  return this.http.patch<{id:string;version:number}>(this.url+'/'+id+'/status',{id,version,isActive});
 }
 delete(id:string,version:number){
  return this.http.delete<void>(this.url+'/'+id,{params:new HttpParams().set('version',version)});
 }
 export(q:DefinitionQuery){return this.http.get(this.url+'/export',{params:this.params(q),responseType:'blob'});}
 
}

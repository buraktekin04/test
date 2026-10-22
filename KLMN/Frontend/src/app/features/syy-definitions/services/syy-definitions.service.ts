import {inject,Injectable} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {API_BASE_URL} from '../../../core/config/api.tokens';
import {DefinitionItem,DefinitionQuery,DiagnosisOptions,PageResult} from '../models/syy-definition.model';
/** Altı bağımsız tanım ekranının tipli API istemcisi. */
@Injectable({providedIn:'root'})
export class SyyDefinitionsService {
 private readonly http=inject(HttpClient);
 private readonly url=inject(API_BASE_URL);
 private endpoint(path:string){return this.url+'/'+path;}
 private params(q:DefinitionQuery){
  let p=new HttpParams().set('pageNumber',q.pageNumber).set('pageSize',q.pageSize)
   .set('includeInactive',q.includeInactive);
  if(q.search.trim())p=p.set('search',q.search.trim());
  if(q.isActive!==null)p=p.set('isActive',q.isActive);
  return p;
 }
 list(path:string,q:DefinitionQuery){return this.http.get<PageResult<DefinitionItem>>(this.endpoint(path),{params:this.params(q)});}
 detail(path:string,id:string){return this.http.get<DefinitionItem>(this.endpoint(path)+'/'+id);}
 create(path:string,data:Record<string,unknown>){return this.http.post<{id:string;version:number}>(this.endpoint(path),data);}
 update(path:string,id:string,data:Record<string,unknown>){return this.http.put<{id:string;version:number}>(this.endpoint(path)+'/'+id,data);}
 status(path:string,id:string,version:number,isActive:boolean){
  return this.http.patch<{id:string;version:number}>(this.endpoint(path)+'/'+id+'/status',{id,version,isActive});
 }
 delete(path:string,id:string,version:number){
  return this.http.delete<void>(this.endpoint(path)+'/'+id,{params:new HttpParams().set('version',version)});
 }
 export(path:string,q:DefinitionQuery){return this.http.get(this.endpoint(path)+'/export',{params:this.params(q),responseType:'blob'});}
 options(id?:string,type?:number|null,slice?:number|null,parent?:string|null){
  let p=new HttpParams();
  if(id)p=p.set('currentId',id);
  if(type!==null&&type!==undefined)p=p.set('currentType',type);
  if(slice!==null&&slice!==undefined)p=p.set('currentSlice',slice);
  if(parent)p=p.set('currentParent',parent);
  return this.http.get<DiagnosisOptions>(this.endpoint('syy-diagnoses')+'/options',{params:p});
 }
}

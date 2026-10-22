import {inject,Injectable} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {API_BASE_URL} from '../../../core/config/api.tokens';
import {SyyDiagnosisItem,DefinitionQuery,PageResult,DiagnosisOptions} from '../models/syy-diagnosis.model';
/** SYYDiagnoses modülünün bağımsız API servisi; ekran adresi kodda sabittir. */
@Injectable({providedIn:'root'})
export class SyyDiagnosisService {
 private readonly http=inject(HttpClient);
 private readonly url=inject(API_BASE_URL)+'/syy-diagnoses';
 private params(q:DefinitionQuery):HttpParams {
  let p=new HttpParams().set('pageNumber',q.pageNumber).set('pageSize',q.pageSize)
   .set('includeInactive',q.includeInactive);
  if(q.search.trim())p=p.set('search',q.search.trim());
  if(q.isActive!==null)p=p.set('isActive',q.isActive);
  return p;
 }
 list(q:DefinitionQuery){return this.http.get<PageResult<SyyDiagnosisItem>>(this.url,{params:this.params(q)});}
 detail(id:string){return this.http.get<SyyDiagnosisItem>(this.url+'/'+id);}
 create(payload:Record<string,unknown>){return this.http.post<{id:string;version:number}>(this.url,payload);}
 update(id:string,payload:Record<string,unknown>){return this.http.put<{id:string;version:number}>(this.url+'/'+id,payload);}
 status(id:string,version:number,isActive:boolean){
  return this.http.patch<{id:string;version:number}>(this.url+'/'+id+'/status',{id,version,isActive});
 }
 delete(id:string,version:number){
  return this.http.delete<void>(this.url+'/'+id,{params:new HttpParams().set('version',version)});
 }
 export(q:DefinitionQuery){return this.http.get(this.url+'/export',{params:this.params(q),responseType:'blob'});}
 /** Bu dropdown sadece SYY tanı yetkisine ihtiyaç duyar. */
 options(id?:string,type?:number|null,slice?:number|null,parent?:string|null){
  let p=new HttpParams();
  if(id)p=p.set('currentId',id);
  if(type!==null&&type!==undefined)p=p.set('currentType',type);
  if(slice!==null&&slice!==undefined)p=p.set('currentSlice',slice);
  if(parent)p=p.set('currentParent',parent);
  return this.http.get<DiagnosisOptions>(this.url+'/options',{params:p});
 }
}

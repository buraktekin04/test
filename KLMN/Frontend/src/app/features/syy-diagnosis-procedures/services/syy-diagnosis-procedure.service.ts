import {Injectable,inject} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {API_BASE_URL} from '../../../core/config/api.tokens';
import {SyyDiagnosisProcedureItem,SyyDiagnosisProcedureItemOptions,SyyDiagnosisProcedureItemQuery,PagedItems} from '../models/syy-diagnosis-procedure.model';
/** SYY Gereken Tetkikler için ayrı CRUD/API servisi. */
@Injectable({providedIn:'root'})
export class SyyDiagnosisProcedureService {
 private readonly http=inject(HttpClient);
 private readonly url=inject(API_BASE_URL)+'/syy-diagnosis-procedures';
 private params(q:SyyDiagnosisProcedureItemQuery){
  let p=new HttpParams().set('pageNumber',q.pageNumber).set('pageSize',q.pageSize)
   .set('includeInactive',q.includeInactive);
  if(q.search.trim())p=p.set('search',q.search.trim());
  if(q.isActive!==null)p=p.set('isActive',q.isActive);
  if(q.syyDiagnosisDefId)p=p.set('syyDiagnosisDefId',q.syyDiagnosisDefId);
  return p;
 }
 list(q:SyyDiagnosisProcedureItemQuery){return this.http.get<PagedItems<SyyDiagnosisProcedureItem>>(this.url,{params:this.params(q)});}
 detail(id:string){return this.http.get<SyyDiagnosisProcedureItem>(this.url+'/'+id);}
 options(selected?:SyyDiagnosisProcedureItem){
  let params=new HttpParams();
  if(selected?.syyDiagnosisDefId)params=params.set('selectedSyyDiagnosisDefId',selected.syyDiagnosisDefId);
  if(selected?.procedureDefId)params=params.set('selectedProcedureDefId',selected.procedureDefId);
  return this.http.get<SyyDiagnosisProcedureItemOptions>(this.url+'/options',{params});
 }
 create(data:Record<string,unknown>){return this.http.post<{id:string;version:number}>(this.url,data);}
 update(id:string,data:Record<string,unknown>){return this.http.put<{id:string;version:number}>(this.url+'/'+id,data);}
 status(item:SyyDiagnosisProcedureItem){return this.http.patch(this.url+'/'+item.id+'/status',
  {id:item.id,version:item.version,isActive:!item.isActive});}
 delete(item:SyyDiagnosisProcedureItem){return this.http.delete(this.url+'/'+item.id,{params:new HttpParams().set('version',item.version)});}
 export(q:SyyDiagnosisProcedureItemQuery){return this.http.get(this.url+'/export',{params:this.params(q),responseType:'blob'});}
}

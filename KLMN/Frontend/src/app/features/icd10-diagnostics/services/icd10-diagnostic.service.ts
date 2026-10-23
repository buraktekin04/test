import {Injectable,inject} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {API_BASE_URL} from '../../../core/config/api.tokens';
import {Icd10DiagnosticItem,Icd10DiagnosticOptions,Icd10DiagnosticQuery,PagedItems} from '../models/icd10-diagnostic.model';
/** ICD10 SYY Tanı İstatistikleri ekranına özel API/CRUD. */
@Injectable({providedIn:'root'})
export class Icd10DiagnosticService {
 private readonly http=inject(HttpClient);
 private readonly url=inject(API_BASE_URL)+'/icd10-diagnostics';
 private params(q:Icd10DiagnosticQuery){
  let p=new HttpParams().set('pageNumber',q.pageNumber).set('pageSize',q.pageSize)
   .set('includeInactive',q.includeInactive);
  if(q.search.trim())p=p.set('search',q.search.trim());
  if(q.isActive!==null)p=p.set('isActive',q.isActive);
  if(q.icd10DefId)p=p.set('icd10DefId',q.icd10DefId);
  if(q.syyDiagnosisDefId)p=p.set('syyDiagnosisDefId',q.syyDiagnosisDefId);
  if(q.hcAdmissionTypeDefId)p=p.set('hcAdmissionTypeDefId',q.hcAdmissionTypeDefId);
  return p;
 }
 list(q:Icd10DiagnosticQuery){return this.http.get<PagedItems<Icd10DiagnosticItem>>(this.url,{params:this.params(q)});}
 detail(id:string){return this.http.get<Icd10DiagnosticItem>(this.url+'/'+id);}
 options(row?:Icd10DiagnosticItem){
  let p=new HttpParams();
  if(row){
   p=p.set('selectedICD10DefId',row.icd10DefId)
    .set('selectedSYYDiagnosisDefId',row.syyDiagnosisDefId)
    .set('selectedHcAdmissionTypeDefId',row.hcAdmissionTypeDefId);
  }
  return this.http.get<Icd10DiagnosticOptions>(this.url+'/options',{params:p});
 }
 create(body:Record<string,unknown>){return this.http.post<{id:string;version:number}>(this.url,body);}
 update(id:string,body:Record<string,unknown>){return this.http.put<{id:string;version:number}>(this.url+'/'+id,body);}
 status(row:Icd10DiagnosticItem){return this.http.patch(this.url+'/'+row.id+'/status',{id:row.id,version:row.version,isActive:!row.isActive});}
 delete(row:Icd10DiagnosticItem){return this.http.delete<void>(this.url+'/'+row.id,{params:new HttpParams().set('version',row.version)});}
 export(q:Icd10DiagnosticQuery){return this.http.get(this.url+'/export',{params:this.params(q),responseType:'blob'});}
}

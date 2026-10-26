import {inject,Injectable} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {API_BASE_URL} from '../../../core/config/api.tokens';
import {SyyDiagnosisDecisionItem,SyyDiagnosisDecisionQuery,SyyDiagnosisDecisionFields,
 SyyDiagnosisDecisionOptions,SyyDiagnosisDecisionPagedResult,SyyDiagnosisDecisionMutation} from '../models/syy-diagnosis-decision.model';
/** İki ayrı ekranın ortak tür güvenli API servisi; backend filtreleri tek kaynaktır. */
@Injectable({providedIn:'root'})
export class SyyDiagnosisDecisionService {
 private readonly http=inject(HttpClient);
 private readonly url=inject(API_BASE_URL)+'/syy-diagnosis-decisions';
 private params(q:SyyDiagnosisDecisionQuery):HttpParams {
  let p=new HttpParams().set('pageNumber',q.pageNumber).set('pageSize',q.pageSize)
    .set('includeInactive',q.includeInactive);
  if(q.search.trim())p=p.set('search',q.search.trim());
  if(q.isActive!==null)p=p.set('isActive',q.isActive);
  if(q.syyDiagnosisDefId)p=p.set('syyDiagnosisDefId',q.syyDiagnosisDefId);
  if(q.hcAdmissionTypeDefId)p=p.set('hcAdmissionTypeDefId',q.hcAdmissionTypeDefId);
  if(q.militaryClassDefId)p=p.set('militaryClassDefId',q.militaryClassDefId);
  if(q.hcDecisionDefId)p=p.set('hcDecisionDefId',q.hcDecisionDefId);
  return p;
 }
 list(q:SyyDiagnosisDecisionQuery){return this.http.get<SyyDiagnosisDecisionPagedResult>(this.url,{params:this.params(q)});}
 search(q:SyyDiagnosisDecisionQuery){return this.http.get<SyyDiagnosisDecisionPagedResult>(this.url+'/search',{params:this.params(q)});}
 detail(id:string){return this.http.get<SyyDiagnosisDecisionItem>(this.url+'/'+id);}
 options(row?:SyyDiagnosisDecisionItem,includeInactive=false){
  let p=new HttpParams().set('includeInactive',includeInactive);
  if(row){
   p=p.set('selectedSYYDiagnosisDefId',row.syyDiagnosisDefId).set('selectedHcDecisionDefId',row.hcDecisionDefId)
    .set('selectedHcAdmissionTypeDefId',row.hcAdmissionTypeDefId);
   if(row.militaryClassDefId)p=p.set('selectedMilitaryClassDefId',row.militaryClassDefId);
  }
  return this.http.get<SyyDiagnosisDecisionOptions>(this.url+'/options',{params:p});
 }
 create(fields:SyyDiagnosisDecisionFields){return this.http.post<SyyDiagnosisDecisionMutation>(this.url,fields);}
 update(record:SyyDiagnosisDecisionFields&{id:string;version:number}){
  return this.http.put<SyyDiagnosisDecisionMutation>(this.url+'/'+record.id,record);
 }
 changeStatus(row:SyyDiagnosisDecisionItem){return this.http.patch<SyyDiagnosisDecisionMutation>(this.url+'/'+row.id+'/status',
  {id:row.id,version:row.version,isActive:!row.isActive});}
 delete(row:SyyDiagnosisDecisionItem){return this.http.delete<void>(this.url+'/'+row.id,{params:new HttpParams().set('version',row.version)});}
 export(q:SyyDiagnosisDecisionQuery){return this.http.get(this.url+'/export',{params:this.params(q),responseType:'blob'});}
}

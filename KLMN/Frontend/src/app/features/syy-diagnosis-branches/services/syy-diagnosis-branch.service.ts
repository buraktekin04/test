import {Injectable,inject} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {API_BASE_URL} from '../../../core/config/api.tokens';
import {SyyDiagnosisBranchItem,SyyDiagnosisBranchItemOptions,SyyDiagnosisBranchItemQuery,PagedItems} from '../models/syy-diagnosis-branch.model';
/** SYY Gereken Branşlar için ayrı CRUD/API servisi. */
@Injectable({providedIn:'root'})
export class SyyDiagnosisBranchService {
 private readonly http=inject(HttpClient);
 private readonly url=inject(API_BASE_URL)+'/syy-diagnosis-branches';
 private params(q:SyyDiagnosisBranchItemQuery){
  let p=new HttpParams().set('pageNumber',q.pageNumber).set('pageSize',q.pageSize)
   .set('includeInactive',q.includeInactive);
  if(q.search.trim())p=p.set('search',q.search.trim());
  if(q.isActive!==null)p=p.set('isActive',q.isActive);
  if(q.syyDiagnosisDefId)p=p.set('syyDiagnosisDefId',q.syyDiagnosisDefId);
  return p;
 }
 list(q:SyyDiagnosisBranchItemQuery){return this.http.get<PagedItems<SyyDiagnosisBranchItem>>(this.url,{params:this.params(q)});}
 detail(id:string){return this.http.get<SyyDiagnosisBranchItem>(this.url+'/'+id);}
 options(selected?:SyyDiagnosisBranchItem){
  let params=new HttpParams();
  if(selected?.syyDiagnosisDefId)params=params.set('selectedSyyDiagnosisDefId',selected.syyDiagnosisDefId);
  if(selected?.branchDefId)params=params.set('selectedBranchDefId',selected.branchDefId);
  return this.http.get<SyyDiagnosisBranchItemOptions>(this.url+'/options',{params});
 }
 create(data:Record<string,unknown>){return this.http.post<{id:string;version:number}>(this.url,data);}
 update(id:string,data:Record<string,unknown>){return this.http.put<{id:string;version:number}>(this.url+'/'+id,data);}
 status(item:SyyDiagnosisBranchItem){return this.http.patch(this.url+'/'+item.id+'/status',
  {id:item.id,version:item.version,isActive:!item.isActive});}
 delete(item:SyyDiagnosisBranchItem){return this.http.delete(this.url+'/'+item.id,{params:new HttpParams().set('version',item.version)});}
 export(q:SyyDiagnosisBranchItemQuery){return this.http.get(this.url+'/export',{params:this.params(q),responseType:'blob'});}
}

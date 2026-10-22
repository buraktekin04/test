import {Injectable,inject} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {API_BASE_URL} from '../../../core/config/api.tokens';
import {SyyClassificationItem,SyyClassificationItemOptions,SyyClassificationItemQuery,PagedItems} from '../models/syy-classification.model';
/** SYY Tanı Sınıflandırma Çizelgesi için ayrı CRUD/API servisi. */
@Injectable({providedIn:'root'})
export class SyyClassificationService {
 private readonly http=inject(HttpClient);
 private readonly url=inject(API_BASE_URL)+'/syy-classifications';
 private params(q:SyyClassificationItemQuery){
  let p=new HttpParams().set('pageNumber',q.pageNumber).set('pageSize',q.pageSize)
   .set('includeInactive',q.includeInactive);
  if(q.search.trim())p=p.set('search',q.search.trim());
  if(q.isActive!==null)p=p.set('isActive',q.isActive);
  if(q.syyDiagnosisDefId)p=p.set('syyDiagnosisDefId',q.syyDiagnosisDefId);
  return p;
 }
 list(q:SyyClassificationItemQuery){return this.http.get<PagedItems<SyyClassificationItem>>(this.url,{params:this.params(q)});}
 detail(id:string){return this.http.get<SyyClassificationItem>(this.url+'/'+id);}
 options(selected?:SyyClassificationItem){
  let params=new HttpParams();
  if(selected?.syyDiagnosisDefId)params=params.set('selectedSyyDiagnosisDefId',selected.syyDiagnosisDefId);
  if(selected?.forcesCommandDef)params=params.set('selectedForcesCommandDef',selected.forcesCommandDef);
  if(selected?.rankGroup)params=params.set('selectedRankGroup',selected.rankGroup);
  if(selected?.militaryClassGroup)params=params.set('selectedMilitaryClassGroup',selected.militaryClassGroup);
  if(selected?.hcDecision)params=params.set('selectedHcDecision',selected.hcDecision);
  return this.http.get<SyyClassificationItemOptions>(this.url+'/options',{params});
 }
 create(data:Record<string,unknown>){return this.http.post<{id:string;version:number}>(this.url,data);}
 update(id:string,data:Record<string,unknown>){return this.http.put<{id:string;version:number}>(this.url+'/'+id,data);}
 status(item:SyyClassificationItem){return this.http.patch(this.url+'/'+item.id+'/status',
  {id:item.id,version:item.version,isActive:!item.isActive});}
 delete(item:SyyClassificationItem){return this.http.delete(this.url+'/'+item.id,{params:new HttpParams().set('version',item.version)});}
 export(q:SyyClassificationItemQuery){return this.http.get(this.url+'/export',{params:this.params(q),responseType:'blob'});}
}

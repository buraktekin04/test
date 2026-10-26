import {inject,Injectable} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {API_BASE_URL} from '../../../core/config/api.tokens';
import {SyyRulePagedResponse,SyyRuleQuery} from '../models/syy-rule.model';
/** Mevcut SYYDiagnosisDef kayıtları ve eşleştirmeleri için yalnız GET servisi. */
@Injectable({providedIn:'root'})
export class SyyRuleService {
 private readonly http=inject(HttpClient);
 private readonly url=inject(API_BASE_URL)+'/syy-rules';
 list(query:SyyRuleQuery){
   let params=new HttpParams().set('search',query.search.trim())
     .set('includeInactive',query.includeInactive).set('pageNumber',query.pageNumber)
     .set('pageSize',query.pageSize);
   if(query.syyDiagnosisDefId)params=params.set('syyDiagnosisDefId',query.syyDiagnosisDefId);
   return this.http.get<SyyRulePagedResponse>(this.url,{params});
 }
}

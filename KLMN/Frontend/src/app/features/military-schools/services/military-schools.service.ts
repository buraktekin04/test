import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  MilitarySchoolDefItem, MilitarySchoolFields, MilitarySchoolQuery,
  UpdateMilitarySchoolDefRequest, ChangeMilitarySchoolDefStatusRequest,
  MilitarySchoolDefMutationResponse, MilitarySchoolOptions
} from '../models/military-schools.model';

/** Askerî Okul Tanımları'nın yetki korumalı HTTP API sözleşmesini kapsar. */
@Injectable({ providedIn: 'root' })
export class MilitarySchoolService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/military-schools';

  /** Filtrelerin tamamını listeleme ve Excel için ortak biçimde üretir. */
  private params(query: MilitarySchoolQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber).set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    if (query.cityId) params = params.set('cityId', query.cityId);
    if (query.forcesCommandDefId) params = params.set('forcesCommandDefId', query.forcesCommandDefId);
    return params;
  }

  public list(query: MilitarySchoolQuery): Observable<PagedResult<MilitarySchoolDefItem>> {
    return this.http.get<PagedResult<MilitarySchoolDefItem>>(this.url, { params: this.params(query) });
  }
  public getById(id: string): Observable<MilitarySchoolDefItem> {
    return this.http.get<MilitarySchoolDefItem>(this.url + '/' + id);
  }
  public create(request: MilitarySchoolFields): Observable<MilitarySchoolDefMutationResponse> {
    return this.http.post<MilitarySchoolDefMutationResponse>(this.url, request);
  }
  public update(request: UpdateMilitarySchoolDefRequest): Observable<MilitarySchoolDefMutationResponse> {
    return this.http.put<MilitarySchoolDefMutationResponse>(this.url + '/' + request.id, request);
  }
  public changeStatus(request: ChangeMilitarySchoolDefStatusRequest): Observable<MilitarySchoolDefMutationResponse> {
    return this.http.patch<MilitarySchoolDefMutationResponse>(this.url + '/' + request.id + '/status', request);
  }
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id,
      { params: new HttpParams().set('version', version) });
  }
  public export(query: MilitarySchoolQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', { params: this.params(query), responseType: 'blob' });
  }

  /** Yalnızca MilitarySchools izniyle iki FK seçim listesini alır. */
  public options(selectedCityId?: string | null,
                 selectedForcesCommandDefId?: string | null): Observable<MilitarySchoolOptions> {
    let params = new HttpParams();
    if (selectedCityId) params = params.set('selectedCityId', selectedCityId);
    if (selectedForcesCommandDefId)
      params = params.set('selectedForcesCommandDefId', selectedForcesCommandDefId);
    return this.http.get<MilitarySchoolOptions>(this.url + '/options', { params });
  }
}

import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  CityDefItem, CityFields, CityQuery,
  UpdateCityDefRequest, ChangeCityDefStatusRequest, CityDefMutationResponse, CityCountryOption
} from '../models/cities.model';

@Injectable({ providedIn: 'root' })
export class CityService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/cities';

  private params(query: CityQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber).set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    if (query.countryId) params = params.set('countryId', query.countryId);
    return params;
  }
  public list(query: CityQuery): Observable<PagedResult<CityDefItem>> {
    return this.http.get<PagedResult<CityDefItem>>(this.url, { params: this.params(query) });
  }
  public getById(id: string): Observable<CityDefItem> {
    return this.http.get<CityDefItem>(this.url + '/' + id);
  }
  public create(request: CityFields): Observable<CityDefMutationResponse> {
    return this.http.post<CityDefMutationResponse>(this.url, request);
  }
  public update(request: UpdateCityDefRequest): Observable<CityDefMutationResponse> {
    return this.http.put<CityDefMutationResponse>(this.url + '/' + request.id, request);
  }
  public changeStatus(request: ChangeCityDefStatusRequest): Observable<CityDefMutationResponse> {
    return this.http.patch<CityDefMutationResponse>(this.url + '/' + request.id + '/status', request);
  }
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, {
      params: new HttpParams().set('version', version)
    });
  }
  public export(query: CityQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', {
      params: this.params(query), responseType: 'blob'
    });
  }
  public countryOptions(selectedCountryId?: string | null): Observable<CityCountryOption[]> {
    const params = selectedCountryId
      ? new HttpParams().set('selectedCountryId', selectedCountryId) : new HttpParams();
    return this.http.get<CityCountryOption[]>(this.url + '/country-options', { params });
  }
}

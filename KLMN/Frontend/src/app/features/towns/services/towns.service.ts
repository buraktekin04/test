import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  TownDefItem, TownFields, TownQuery,
  UpdateTownDefRequest, ChangeTownDefStatusRequest, TownDefMutationResponse, TownCityOption
} from '../models/towns.model';

/**
 * İlçe Tanımları API servisidir. Liste filtrelerini sunucuda uygular;
 * Create/Update/Delete/Excel işlemleri kendi backend izinleriyle korunur.
 */
@Injectable({ providedIn: 'root' })
export class TownService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/towns';

  /** Filtreleri sunucudaki GetTownDefsQuery sözleşmesine dönüştürür. */
  private params(query: TownQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber).set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    if (query.cityId) params = params.set('cityId', query.cityId);
    return params;
  }
  public list(query: TownQuery): Observable<PagedResult<TownDefItem>> {
    return this.http.get<PagedResult<TownDefItem>>(this.url, { params: this.params(query) });
  }
  public getById(id: string): Observable<TownDefItem> {
    return this.http.get<TownDefItem>(this.url + '/' + id);
  }
  public create(request: TownFields): Observable<TownDefMutationResponse> {
    return this.http.post<TownDefMutationResponse>(this.url, request);
  }
  public update(request: UpdateTownDefRequest): Observable<TownDefMutationResponse> {
    return this.http.put<TownDefMutationResponse>(this.url + '/' + request.id, request);
  }
  public changeStatus(request: ChangeTownDefStatusRequest): Observable<TownDefMutationResponse> {
    return this.http.patch<TownDefMutationResponse>(this.url + '/' + request.id + '/status', request);
  }
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, {
      params: new HttpParams().set('version', version)
    });
  }
  public export(query: TownQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', {
      params: this.params(query), responseType: 'blob'
    });
  }
  /** İlçe modülü izniyle şehir dropdown'unu ve önceki seçili şehri alır. */
  public cityOptions(selectedCityId?: string | null): Observable<TownCityOption[]> {
    const params = selectedCityId
      ? new HttpParams().set('selectedCityId', selectedCityId) : new HttpParams();
    return this.http.get<TownCityOption[]>(this.url + '/city-options', { params });
  }
}

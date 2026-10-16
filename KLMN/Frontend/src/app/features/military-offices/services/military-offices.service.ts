import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  MilitaryOfficeDefItem, MilitaryOfficeFields, MilitaryOfficeQuery,
  UpdateMilitaryOfficeDefRequest, ChangeMilitaryOfficeDefStatusRequest,
  MilitaryOfficeDefMutationResponse, MilitaryOfficeOption
} from '../models/military-offices.model';

/** Şube tanımlarını mevcut diğer tanımlarla aynı API sözleşmesinde yönetir. */
@Injectable({ providedIn: 'root' })
export class MilitaryOfficeService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/military-offices';

  /** Sunucuda liste ve Excel için aynı filtreleri uygular. */
  private params(query: MilitaryOfficeQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber).set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    if (query.cityId) params = params.set('cityId', query.cityId);
    return params;
  }

  public list(query: MilitaryOfficeQuery): Observable<PagedResult<MilitaryOfficeDefItem>> {
    return this.http.get<PagedResult<MilitaryOfficeDefItem>>(this.url, { params: this.params(query) });
  }
  public getById(id: string): Observable<MilitaryOfficeDefItem> {
    return this.http.get<MilitaryOfficeDefItem>(this.url + '/' + id);
  }
  public create(request: MilitaryOfficeFields): Observable<MilitaryOfficeDefMutationResponse> {
    return this.http.post<MilitaryOfficeDefMutationResponse>(this.url, request);
  }
  public update(request: UpdateMilitaryOfficeDefRequest): Observable<MilitaryOfficeDefMutationResponse> {
    return this.http.put<MilitaryOfficeDefMutationResponse>(this.url + '/' + request.id, request);
  }
  public changeStatus(request: ChangeMilitaryOfficeDefStatusRequest): Observable<MilitaryOfficeDefMutationResponse> {
    return this.http.patch<MilitaryOfficeDefMutationResponse>(this.url + '/' + request.id + '/status', request);
  }
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id,
      { params: new HttpParams().set('version', version) });
  }
  public export(query: MilitaryOfficeQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', { params: this.params(query), responseType: 'blob' });
  }

  /** Şube yetkisiyle şehir seçimini yükler; Cities izni ayrıca aranmaz. */
  public options(selectedCityId?: string | null): Observable<MilitaryOfficeOption[]> {
    let params = new HttpParams();
    if (selectedCityId) params = params.set('selectedCityId', selectedCityId);
    return this.http.get<MilitaryOfficeOption[]>(this.url + '/options', { params });
  }
}

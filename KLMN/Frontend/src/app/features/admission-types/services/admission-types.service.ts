import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  AdmissionTypeDefItem, AdmissionTypeFields, AdmissionTypeQuery,
  UpdateAdmissionTypeRequest, ChangeAdmissionTypeStatusRequest,
  AdmissionTypeMutationResponse
} from '../models/admission-types.model';

/** Yetki kontrollü Başvuru Grubu Tanımları HTTP API istemcisi. */
@Injectable({ providedIn: 'root' })
export class AdmissionTypesService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/admission-types';

  /** Ekran ve Excel isteği için aynı sunucu filtrelerini oluşturur. */
  private params(query: AdmissionTypeQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber).set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    return params;
  }

  public list(query: AdmissionTypeQuery): Observable<PagedResult<AdmissionTypeDefItem>> {
    return this.http.get<PagedResult<AdmissionTypeDefItem>>(this.url, { params: this.params(query) });
  }
  public getById(id: string): Observable<AdmissionTypeDefItem> {
    return this.http.get<AdmissionTypeDefItem>(this.url + '/' + id);
  }
  public create(request: AdmissionTypeFields): Observable<AdmissionTypeMutationResponse> {
    return this.http.post<AdmissionTypeMutationResponse>(this.url, request);
  }
  public update(request: UpdateAdmissionTypeRequest): Observable<AdmissionTypeMutationResponse> {
    return this.http.put<AdmissionTypeMutationResponse>(this.url + '/' + request.id, request);
  }
  public changeStatus(request: ChangeAdmissionTypeStatusRequest): Observable<AdmissionTypeMutationResponse> {
    return this.http.patch<AdmissionTypeMutationResponse>(this.url + '/' + request.id + '/status', request);
  }
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, { params: new HttpParams().set('version', version) });
  }
  public export(query: AdmissionTypeQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', { params: this.params(query), responseType: 'blob' });
  }
}

import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  ReportTypeDefItem, ReportTypeFields, ReportTypeQuery,
  UpdateReportTypeDefRequest, ChangeReportTypeDefStatusRequest,
  ReportTypeDefMutationResponse
} from '../models/report-types.model';

/** Rapor Türü Tanımları için CRUD, listeleme ve Excel endpoint istemcisidir. */
@Injectable({ providedIn: 'root' })
export class ReportTypeService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/report-types';

  /** BaseEntity.IsActive filtresini API sorgusuna ekler. */
  private params(query: ReportTypeQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber).set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    return params;
  }
  public list(query: ReportTypeQuery): Observable<PagedResult<ReportTypeDefItem>> {
    return this.http.get<PagedResult<ReportTypeDefItem>>(this.url, { params: this.params(query) });
  }
  public getById(id: string): Observable<ReportTypeDefItem> {
    return this.http.get<ReportTypeDefItem>(this.url + '/' + id);
  }
  public create(request: ReportTypeFields): Observable<ReportTypeDefMutationResponse> {
    return this.http.post<ReportTypeDefMutationResponse>(this.url, request);
  }
  public update(request: UpdateReportTypeDefRequest): Observable<ReportTypeDefMutationResponse> {
    return this.http.put<ReportTypeDefMutationResponse>(this.url + '/' + request.id, request);
  }
  public changeStatus(request: ChangeReportTypeDefStatusRequest): Observable<ReportTypeDefMutationResponse> {
    return this.http.patch<ReportTypeDefMutationResponse>(this.url + '/' + request.id + '/status', request);
  }
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, { params: new HttpParams().set('version', version) });
  }
  /** Export yalnızca Query izniyle sunucuda yetkilendirilir. */
  public export(query: ReportTypeQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', { params: this.params(query), responseType: 'blob' });
  }
}

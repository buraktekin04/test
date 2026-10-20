import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  ProcedureDefItem, ProcedureFields, ProcedureQuery,
  UpdateProcedureDefRequest, ChangeProcedureDefStatusRequest,
  ProcedureDefMutationResponse
} from '../models/procedures.model';

/** Tetkik Tanımları için CRUD, listeleme ve Excel endpoint istemcisidir. */
@Injectable({ providedIn: 'root' })
export class ProcedureService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/procedures';

  /** BaseEntity.IsActive filtresini API sorgusuna ekler. */
  private params(query: ProcedureQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber).set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    return params;
  }
  public list(query: ProcedureQuery): Observable<PagedResult<ProcedureDefItem>> {
    return this.http.get<PagedResult<ProcedureDefItem>>(this.url, { params: this.params(query) });
  }
  public getById(id: string): Observable<ProcedureDefItem> {
    return this.http.get<ProcedureDefItem>(this.url + '/' + id);
  }
  public create(request: ProcedureFields): Observable<ProcedureDefMutationResponse> {
    return this.http.post<ProcedureDefMutationResponse>(this.url, request);
  }
  public update(request: UpdateProcedureDefRequest): Observable<ProcedureDefMutationResponse> {
    return this.http.put<ProcedureDefMutationResponse>(this.url + '/' + request.id, request);
  }
  public changeStatus(request: ChangeProcedureDefStatusRequest): Observable<ProcedureDefMutationResponse> {
    return this.http.patch<ProcedureDefMutationResponse>(this.url + '/' + request.id + '/status', request);
  }
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, { params: new HttpParams().set('version', version) });
  }
  /** Export yalnızca Query izniyle sunucuda yetkilendirilir. */
  public export(query: ProcedureQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', { params: this.params(query), responseType: 'blob' });
  }
}

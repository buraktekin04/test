import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  ConnectedDecisionDefItem, ConnectedDecisionFields, ConnectedDecisionQuery,
  UpdateConnectedDecisionDefRequest, ChangeConnectedDecisionDefStatusRequest,
  ConnectedDecisionDefMutationResponse
} from '../models/connected-decisions.model';

/** Bağlı Kurul Kararı Tanımları için CRUD, listeleme ve Excel endpoint istemcisidir. */
@Injectable({ providedIn: 'root' })
export class ConnectedDecisionService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/connected-decisions';

  /** BaseEntity.IsActive filtresini API sorgusuna ekler. */
  private params(query: ConnectedDecisionQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber).set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    return params;
  }
  public list(query: ConnectedDecisionQuery): Observable<PagedResult<ConnectedDecisionDefItem>> {
    return this.http.get<PagedResult<ConnectedDecisionDefItem>>(this.url, { params: this.params(query) });
  }
  public getById(id: string): Observable<ConnectedDecisionDefItem> {
    return this.http.get<ConnectedDecisionDefItem>(this.url + '/' + id);
  }
  public create(request: ConnectedDecisionFields): Observable<ConnectedDecisionDefMutationResponse> {
    return this.http.post<ConnectedDecisionDefMutationResponse>(this.url, request);
  }
  public update(request: UpdateConnectedDecisionDefRequest): Observable<ConnectedDecisionDefMutationResponse> {
    return this.http.put<ConnectedDecisionDefMutationResponse>(this.url + '/' + request.id, request);
  }
  public changeStatus(request: ChangeConnectedDecisionDefStatusRequest): Observable<ConnectedDecisionDefMutationResponse> {
    return this.http.patch<ConnectedDecisionDefMutationResponse>(this.url + '/' + request.id + '/status', request);
  }
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, { params: new HttpParams().set('version', version) });
  }
  /** Export yalnızca Query izniyle sunucuda yetkilendirilir. */
  public export(query: ConnectedDecisionQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', { params: this.params(query), responseType: 'blob' });
  }
}

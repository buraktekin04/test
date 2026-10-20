import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  RankGroupDefItem, RankGroupFields, RankGroupQuery,
  UpdateRankGroupDefRequest, ChangeRankGroupDefStatusRequest,
  RankGroupDefMutationResponse
} from '../models/rank-groups.model';

/** Rütbe Grubu Tanımları için CRUD, listeleme ve Excel endpoint istemcisidir. */
@Injectable({ providedIn: 'root' })
export class RankGroupService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/rank-groups';

  /** BaseEntity.IsActive filtresini API sorgusuna ekler. */
  private params(query: RankGroupQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber).set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    return params;
  }
  public list(query: RankGroupQuery): Observable<PagedResult<RankGroupDefItem>> {
    return this.http.get<PagedResult<RankGroupDefItem>>(this.url, { params: this.params(query) });
  }
  public getById(id: string): Observable<RankGroupDefItem> {
    return this.http.get<RankGroupDefItem>(this.url + '/' + id);
  }
  public create(request: RankGroupFields): Observable<RankGroupDefMutationResponse> {
    return this.http.post<RankGroupDefMutationResponse>(this.url, request);
  }
  public update(request: UpdateRankGroupDefRequest): Observable<RankGroupDefMutationResponse> {
    return this.http.put<RankGroupDefMutationResponse>(this.url + '/' + request.id, request);
  }
  public changeStatus(request: ChangeRankGroupDefStatusRequest): Observable<RankGroupDefMutationResponse> {
    return this.http.patch<RankGroupDefMutationResponse>(this.url + '/' + request.id + '/status', request);
  }
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, { params: new HttpParams().set('version', version) });
  }
  /** Export yalnızca Query izniyle sunucuda yetkilendirilir. */
  public export(query: RankGroupQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', { params: this.params(query), responseType: 'blob' });
  }
}

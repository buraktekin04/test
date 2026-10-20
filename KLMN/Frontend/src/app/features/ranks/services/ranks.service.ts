import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  RankDefItem, RankFields, RankQuery,
  UpdateRankDefRequest, ChangeRankDefStatusRequest,
  RankDefMutationResponse, RankOption
  /** Rütbe ekranı yetkisiyle aktif grupları yükler; eski pasif seçim düzenlemede korunur. */
  public options(selectedRankGroupId?: string | null): Observable<RankOption[]> {
    let params = new HttpParams();
    if (selectedRankGroupId) params = params.set('selectedRankGroupId', selectedRankGroupId);
    return this.http.get<RankOption[]>(this.url + '/options', {params});
  }
} from '../models/ranks.model';

/** Rütbe Tanımları için CRUD, listeleme ve Excel endpoint istemcisidir. */
@Injectable({ providedIn: 'root' })
export class RankService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/ranks';

  /** BaseEntity.IsActive filtresini API sorgusuna ekler. */
  private params(query: RankQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber).set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    return params;
  }
  public list(query: RankQuery): Observable<PagedResult<RankDefItem>> {
    return this.http.get<PagedResult<RankDefItem>>(this.url, { params: this.params(query) });
  }
  public getById(id: string): Observable<RankDefItem> {
    return this.http.get<RankDefItem>(this.url + '/' + id);
  }
  public create(request: RankFields): Observable<RankDefMutationResponse> {
    return this.http.post<RankDefMutationResponse>(this.url, request);
  }
  public update(request: UpdateRankDefRequest): Observable<RankDefMutationResponse> {
    return this.http.put<RankDefMutationResponse>(this.url + '/' + request.id, request);
  }
  public changeStatus(request: ChangeRankDefStatusRequest): Observable<RankDefMutationResponse> {
    return this.http.patch<RankDefMutationResponse>(this.url + '/' + request.id + '/status', request);
  }
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, { params: new HttpParams().set('version', version) });
  }
  /** Export yalnızca Query izniyle sunucuda yetkilendirilir. */
  public export(query: RankQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', { params: this.params(query), responseType: 'blob' });
  }
}

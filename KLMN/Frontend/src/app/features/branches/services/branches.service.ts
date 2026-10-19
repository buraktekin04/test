import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  BranchDefItem, BranchFields, BranchQuery,
  UpdateBranchDefRequest, ChangeBranchDefStatusRequest,
  BranchDefMutationResponse
} from '../models/branches.model';

/** Branş Tanımları için CRUD, listeleme ve Excel endpoint istemcisidir. */
@Injectable({ providedIn: 'root' })
export class BranchService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/branches';

  /** BaseEntity.IsActive filtresini API sorgusuna ekler. */
  private params(query: BranchQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber).set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    return params;
  }
  public list(query: BranchQuery): Observable<PagedResult<BranchDefItem>> {
    return this.http.get<PagedResult<BranchDefItem>>(this.url, { params: this.params(query) });
  }
  public getById(id: string): Observable<BranchDefItem> {
    return this.http.get<BranchDefItem>(this.url + '/' + id);
  }
  public create(request: BranchFields): Observable<BranchDefMutationResponse> {
    return this.http.post<BranchDefMutationResponse>(this.url, request);
  }
  public update(request: UpdateBranchDefRequest): Observable<BranchDefMutationResponse> {
    return this.http.put<BranchDefMutationResponse>(this.url + '/' + request.id, request);
  }
  public changeStatus(request: ChangeBranchDefStatusRequest): Observable<BranchDefMutationResponse> {
    return this.http.patch<BranchDefMutationResponse>(this.url + '/' + request.id + '/status', request);
  }
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, { params: new HttpParams().set('version', version) });
  }
  /** Export yalnızca Query izniyle sunucuda yetkilendirilir. */
  public export(query: BranchQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', { params: this.params(query), responseType: 'blob' });
  }
}

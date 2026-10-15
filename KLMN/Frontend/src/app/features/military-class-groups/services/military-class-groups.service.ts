import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import { MilitaryClassGroupItem, MilitaryClassGroupFields, MilitaryClassGroupQuery,
  UpdateMilitaryClassGroupRequest, ChangeMilitaryClassGroupStatusRequest, MilitaryClassGroupMutationResponse,
  MilitaryClassGroupOption } from '../models/military-class-groups.model';

/** MilitaryClassGroups HTTP API'sini, yetkilendirilmiş ayrı CRUD uçlarıyla kullanır. */
@Injectable({ providedIn: 'root' })
export class MilitaryClassGroupService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/military-class-groups';

  /** Liste ve Excel'de ortak filtreleri kullanır. */
  private params(query: MilitaryClassGroupQuery): HttpParams {
    let params = new HttpParams().set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize).set('includeInactive', query.includeInactive);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    if (query.forcesCommandId) params = params.set('forcesCommandId', query.forcesCommandId);
    return params;
  }
  public list(query: MilitaryClassGroupQuery): Observable<PagedResult<MilitaryClassGroupItem>> {
    return this.http.get<PagedResult<MilitaryClassGroupItem>>(this.url, { params: this.params(query) });
  }
  public getById(id: string): Observable<MilitaryClassGroupItem> {
    return this.http.get<MilitaryClassGroupItem>(this.url + '/' + id);
  }
  public create(fields: MilitaryClassGroupFields): Observable<MilitaryClassGroupMutationResponse> {
    return this.http.post<MilitaryClassGroupMutationResponse>(this.url, fields);
  }
  public update(request: UpdateMilitaryClassGroupRequest): Observable<MilitaryClassGroupMutationResponse> {
    return this.http.put<MilitaryClassGroupMutationResponse>(this.url + '/' + request.id, request);
  }
  public changeStatus(request: ChangeMilitaryClassGroupStatusRequest): Observable<MilitaryClassGroupMutationResponse> {
    return this.http.patch<MilitaryClassGroupMutationResponse>(this.url + '/' + request.id + '/status', request);
  }
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, { params: new HttpParams().set('version', version) });
  }
  public export(query: MilitaryClassGroupQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', { params: this.params(query), responseType: 'blob' });
  }
  /** İlişkili tanımlar için ayrıca yönetim izni gerektirmeyen dropdown uç noktası. */
  public options(selectedForceId?: string | null): Observable<MilitaryClassGroupOption[]> {
    let params = new HttpParams();
    if (selectedForceId) params = params.set('selectedForceId', selectedForceId);
    return this.http.get<MilitaryClassGroupOption[]>(this.url + '/options', { params });
  }
}

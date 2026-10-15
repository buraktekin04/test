import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import { MilitaryClassGroupRelationItem, MilitaryClassGroupRelationFields, MilitaryClassGroupRelationQuery,
  UpdateMilitaryClassGroupRelationRequest, ChangeMilitaryClassGroupRelationStatusRequest, MilitaryClassGroupRelationMutationResponse,
  MilitaryClassGroupRelationOptions } from '../models/military-class-group-relations.model';

/** MilitaryClassGroupRelations HTTP API'sini, yetkilendirilmiş ayrı CRUD uçlarıyla kullanır. */
@Injectable({ providedIn: 'root' })
export class MilitaryClassGroupRelationService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/military-class-group-relations';

  /** Liste ve Excel'de ortak filtreleri kullanır. */
  private params(query: MilitaryClassGroupRelationQuery): HttpParams {
    let params = new HttpParams().set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize).set('includeInactive', query.includeInactive);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    if (query.forcesCommandId) params = params.set('forcesCommandId', query.forcesCommandId);
    if (query.militaryClassId) params = params.set('militaryClassId', query.militaryClassId);
    if (query.militaryClassGroupId) params = params.set('militaryClassGroupId', query.militaryClassGroupId);
    return params;
  }
  public list(query: MilitaryClassGroupRelationQuery): Observable<PagedResult<MilitaryClassGroupRelationItem>> {
    return this.http.get<PagedResult<MilitaryClassGroupRelationItem>>(this.url, { params: this.params(query) });
  }
  public getById(id: string): Observable<MilitaryClassGroupRelationItem> {
    return this.http.get<MilitaryClassGroupRelationItem>(this.url + '/' + id);
  }
  public create(fields: MilitaryClassGroupRelationFields): Observable<MilitaryClassGroupRelationMutationResponse> {
    return this.http.post<MilitaryClassGroupRelationMutationResponse>(this.url, fields);
  }
  public update(request: UpdateMilitaryClassGroupRelationRequest): Observable<MilitaryClassGroupRelationMutationResponse> {
    return this.http.put<MilitaryClassGroupRelationMutationResponse>(this.url + '/' + request.id, request);
  }
  public changeStatus(request: ChangeMilitaryClassGroupRelationStatusRequest): Observable<MilitaryClassGroupRelationMutationResponse> {
    return this.http.patch<MilitaryClassGroupRelationMutationResponse>(this.url + '/' + request.id + '/status', request);
  }
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, { params: new HttpParams().set('version', version) });
  }
  public export(query: MilitaryClassGroupRelationQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', { params: this.params(query), responseType: 'blob' });
  }
  /** İlişkili tanımlar için ayrıca yönetim izni gerektirmeyen dropdown uç noktası. */
  public options(forcesCommandId?: string | null, selectedForceId?: string | null, selectedClassId?: string | null, selectedGroupId?: string | null): Observable<MilitaryClassGroupRelationOptions> {
    let params = new HttpParams();
    if (forcesCommandId) params = params.set('forcesCommandId', forcesCommandId);
    if (selectedForceId) params = params.set('selectedForceId', selectedForceId);
    if (selectedClassId) params = params.set('selectedClassId', selectedClassId);
    if (selectedGroupId) params = params.set('selectedGroupId', selectedGroupId);
    return this.http.get<MilitaryClassGroupRelationOptions>(this.url + '/options', { params });
  }
}

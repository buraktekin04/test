import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  HospitalDefItem, HospitalFields, HospitalQuery,
  UpdateHospitalDefRequest, ChangeHospitalDefStatusRequest,
  HospitalDefMutationResponse, HospitalOptions
} from '../models/hospitals.model';

/** Sağlık Tesisi Tanımları için CRUD, listeleme ve Excel endpoint istemcisidir. */
@Injectable({ providedIn: 'root' })
export class HospitalService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/hospitals';

  /** BaseEntity.IsActive filtresini API sorgusuna ekler. */
  private params(query: HospitalQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber).set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    return params;
  }
  public list(query: HospitalQuery): Observable<PagedResult<HospitalDefItem>> {
    return this.http.get<PagedResult<HospitalDefItem>>(this.url, { params: this.params(query) });
  }
  public getById(id: string): Observable<HospitalDefItem> {
    return this.http.get<HospitalDefItem>(this.url + '/' + id);
  }
  public create(request: HospitalFields): Observable<HospitalDefMutationResponse> {
    return this.http.post<HospitalDefMutationResponse>(this.url, request);
  }
  public update(request: UpdateHospitalDefRequest): Observable<HospitalDefMutationResponse> {
    return this.http.put<HospitalDefMutationResponse>(this.url + '/' + request.id, request);
  }
  public changeStatus(request: ChangeHospitalDefStatusRequest): Observable<HospitalDefMutationResponse> {
    return this.http.patch<HospitalDefMutationResponse>(this.url + '/' + request.id + '/status', request);
  }
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, { params: new HttpParams().set('version', version) });
  }
  /** Export yalnızca Query izniyle sunucuda yetkilendirilir. */
  public export(query: HospitalQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', { params: this.params(query), responseType: 'blob' });
  }

  /** Yetkisi yalnızca Hastaneler modülü olan kullanıcı için seçimleri getirir. */
  public options(cityId?: string | null, refereeHospitalId?: string | null): Observable<HospitalOptions> {
    let params = new HttpParams();
    if (cityId) params = params.set('selectedCityId', cityId);
    if (refereeHospitalId) params = params.set('selectedRefereeHospitalId', refereeHospitalId);
    return this.http.get<HospitalOptions>(this.url + '/options', { params });
  }
}

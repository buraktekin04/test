import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  HospitalReportTypeItem, HospitalReportTypeFields, HospitalReportTypeQuery,
  UpdateHospitalReportTypeRequest, ChangeHospitalReportTypeStatusRequest,
  HospitalReportTypeMutationResponse, HospitalReportTypeOptions, HospitalReportTypeGroup
} from '../models/hospital-report-types.model';

/** Sağlık Tesisi Rapor Türü Tanımları için CRUD, listeleme ve Excel endpoint istemcisidir. */
@Injectable({ providedIn: 'root' })
export class HospitalReportTypeService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/hospital-report-types';

  /** BaseEntity.IsActive filtresini API sorgusuna ekler. */
  private params(query: HospitalReportTypeQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber).set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    return params;
  }
  public list(query: HospitalReportTypeQuery): Observable<PagedResult<HospitalReportTypeItem>> {
    return this.http.get<PagedResult<HospitalReportTypeItem>>(this.url, { params: this.params(query) });
  }
  /** Sağlık tesislerini tekilleştiren ve raporları birlikte döndüren liste. */
  public groups(query: HospitalReportTypeQuery): Observable<PagedResult<HospitalReportTypeGroup>> {
    return this.http.get<PagedResult<HospitalReportTypeGroup>>(this.url + '/groups',
      { params: this.params(query) });
  }
  public getById(id: string): Observable<HospitalReportTypeItem> {
    return this.http.get<HospitalReportTypeItem>(this.url + '/' + id);
  }
  public create(request: HospitalReportTypeFields): Observable<HospitalReportTypeMutationResponse> {
    return this.http.post<HospitalReportTypeMutationResponse>(this.url, request);
  }
  public update(request: UpdateHospitalReportTypeRequest): Observable<HospitalReportTypeMutationResponse> {
    return this.http.put<HospitalReportTypeMutationResponse>(this.url + '/' + request.id, request);
  }
  public changeStatus(request: ChangeHospitalReportTypeStatusRequest): Observable<HospitalReportTypeMutationResponse> {
    return this.http.patch<HospitalReportTypeMutationResponse>(this.url + '/' + request.id + '/status', request);
  }
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, { params: new HttpParams().set('version', version) });
  }
  /** Export yalnızca Query izniyle sunucuda yetkilendirilir. */
  public export(query: HospitalReportTypeQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', { params: this.params(query), responseType: 'blob' });
  }

  /** Aktif seçimleri ve eski pasif seçimleri ekranın kendi yetkisiyle getirir. */
  public options(hospitalId?: string | null, reportId?: string | null): Observable<HospitalReportTypeOptions> {
    let params = new HttpParams();
    if (hospitalId) params = params.set('selectedHospitalDefId', hospitalId);
    if (reportId) params = params.set('selectedReportTypeDefId', reportId);
    return this.http.get<HospitalReportTypeOptions>(this.url + '/options', { params });
  }
}

import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  ChangeOrganizationUnitStatusRequest,
  OrganizationParentOption, OrganizationUnitFields,
  OrganizationUnitItem, OrganizationUnitMutationResponse, OrganizationUnitQuery,
  UpdateOrganizationUnitRequest
} from '../models/organization-unit.model';

/** Organizasyon CRUD / seçenek / Excel API işlemlerinin merkezi Angular servisidir. */
@Injectable({ providedIn: 'root' })
export class OrganizationService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/organizations';

  /** Filtreleme ve Excel'in aynı query parametrelerini göndermesini sağlar. */
  private params(query: OrganizationUnitQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);

    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    if (query.rootOnly !== null) params = params.set('rootOnly', query.rootOnly);
    return params;
  }

  /** Bütün kayıtlar üzerinde filtrelenmiş sayfalı listeyi döndürür. */
  public list(query: OrganizationUnitQuery): Observable<PagedResult<OrganizationUnitItem>> {
    return this.http.get<PagedResult<OrganizationUnitItem>>(this.url, {
      params: this.params(query)
    });
  }

  /** Kayıt düzenleme öncesi güncel xmin bilgisiyle detay sorgular. */
  public getById(id: string): Observable<OrganizationUnitItem> {
    return this.http.get<OrganizationUnitItem>(this.url + '/' + id);
  }

  /** Yalnızca aktif üst birim seçeneklerini döndürür. */
  public parentOptions(): Observable<OrganizationParentOption[]> {
    return this.http.get<OrganizationParentOption[]>(this.url + '/parent-options');
  }

  /** Filtreye uygun tüm birimleri gerçek XLSX olarak indirir. */
  public export(query: OrganizationUnitQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', {
      params: this.params(query), responseType: 'blob'
    });
  }

  /** Yeni birim oluşturur. */
  public create(request: OrganizationUnitFields):
    Observable<OrganizationUnitMutationResponse> {
    return this.http.post<OrganizationUnitMutationResponse>(this.url, request);
  }

  /** Birim ve üst birim ilişkisini xmin ile güvenli şekilde günceller. */
  public update(request: UpdateOrganizationUnitRequest):
    Observable<OrganizationUnitMutationResponse> {
    return this.http.put<OrganizationUnitMutationResponse>(
      this.url + '/' + request.id, request
    );
  }

  /** Aktif/pasif durumunu backend ilişki kurallarıyla değiştirir. */
  public changeStatus(request: ChangeOrganizationUnitStatusRequest):
    Observable<OrganizationUnitMutationResponse> {
    return this.http.patch<OrganizationUnitMutationResponse>(
      this.url + '/' + request.id + '/status', request
    );
  }

  /** Bağlı kullanıcısı/alt birimi olmayan birimi soft delete eder. */
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, {
      params: new HttpParams().set('version', version)
    });
  }
}

import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  ChangeMilitaryUnitDefStatusRequest,
  MilitaryUnitParentOption, MilitaryUnitDefFields,
  MilitaryUnitDefItem, MilitaryUnitDefMutationResponse, MilitaryUnitDefQuery,
  UpdateMilitaryUnitDefRequest
} from '../models/military-unit-def.model';

/** Birlik CRUD / seçenek / Excel API işlemlerinin merkezi Angular servisidir. */
@Injectable({ providedIn: 'root' })
export class MilitaryUnitService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/military-units';

  /** Filtreleme ve Excel'in aynı query parametrelerini göndermesini sağlar. */
  private params(query: MilitaryUnitDefQuery): HttpParams {
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
  public list(query: MilitaryUnitDefQuery): Observable<PagedResult<MilitaryUnitDefItem>> {
    return this.http.get<PagedResult<MilitaryUnitDefItem>>(this.url, {
      params: this.params(query)
    });
  }

  /** Kayıt düzenleme öncesi güncel xmin bilgisiyle detay sorgular. */
  public getById(id: string): Observable<MilitaryUnitDefItem> {
    return this.http.get<MilitaryUnitDefItem>(this.url + '/' + id);
  }

  /** Yalnızca aktif üst birlik seçeneklerini döndürür. */
  public parentOptions(): Observable<MilitaryUnitParentOption[]> {
    return this.http.get<MilitaryUnitParentOption[]>(this.url + '/parent-options');
  }

  /** Filtreye uygun tüm birlikleri gerçek XLSX olarak indirir. */
  public export(query: MilitaryUnitDefQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', {
      params: this.params(query), responseType: 'blob'
    });
  }

  /** Yeni birlik oluşturur. */
  public create(request: MilitaryUnitDefFields):
    Observable<MilitaryUnitDefMutationResponse> {
    return this.http.post<MilitaryUnitDefMutationResponse>(this.url, request);
  }

  /** Birlik ve üst birlik ilişkisini xmin ile güvenli şekilde günceller. */
  public update(request: UpdateMilitaryUnitDefRequest):
    Observable<MilitaryUnitDefMutationResponse> {
    return this.http.put<MilitaryUnitDefMutationResponse>(
      this.url + '/' + request.id, request
    );
  }

  /** Aktif/pasif durumunu backend ilişki kurallarıyla değiştirir. */
  public changeStatus(request: ChangeMilitaryUnitDefStatusRequest):
    Observable<MilitaryUnitDefMutationResponse> {
    return this.http.patch<MilitaryUnitDefMutationResponse>(
      this.url + '/' + request.id + '/status', request
    );
  }

  /** Bağlı kullanıcısı/alt birliki olmayan birliki soft delete eder. */
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, {
      params: new HttpParams().set('version', version)
    });
  }
}

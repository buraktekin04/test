import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  ChangeRoleStatusRequest,
  RoleFields,
  RoleItem,
  RoleMutationResponse,
  RolePermissionOption,
  RoleQuery,
  UpdateRolePermissionsRequest,
  UpdateRoleRequest
} from '../models/role.model';

/** Rol Tanımları ekranının tip güvenli API servisidir; access token interceptor'dan eklenir. */
@Injectable({ providedIn: 'root' })
export class RoleService {
  /** Backend HTTP işlemleri için HttpClient servisidir. */
  private readonly http = inject(HttpClient);
  /** Çalışılan ortamın HTTPS API temel adresidir. */
  private readonly url = inject(API_BASE_URL) + '/roles';

  /** Liste ve Excel'de aynı sunucu filtrelerini kullanır. */
  private buildParams(query: RoleQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize)
      .set('sortField', query.sortField)
      .set('sortOrder', query.sortOrder)
      .set('includeInactive', query.includeInactive);

    if (query.search.trim())
      params = params.set('search', query.search.trim());
    if (query.isActive !== null)
      params = params.set('isActive', query.isActive);
    if (query.isSystemRole !== null)
      params = params.set('isSystemRole', query.isSystemRole);

    return params;
  }

  /** Listeyi mevcut filtrelerle backend'den sayfalı getirir. */
  public list(query: RoleQuery): Observable<PagedResult<RoleItem>> {
    return this.http.get<PagedResult<RoleItem>>(this.url, {
      params: this.buildParams(query)
    });
  }

  /** Düzenleme ve izin kaydetmeden önce güncel rol versiyonunu sorgular. */
  public getById(id: string): Observable<RoleItem> {
    return this.http.get<RoleItem>(this.url + '/' + id);
  }

  /** Tüm filtrelenmiş kayıtları gerçek Excel formatında indirir. */
  public export(query: RoleQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', {
      params: this.buildParams(query),
      responseType: 'blob'
    });
  }

  /** Yeni özel rol ekler. */
  public create(data: RoleFields): Observable<RoleMutationResponse> {
    return this.http.post<RoleMutationResponse>(this.url, data);
  }

  /** Kaydı xmin ile güvenli şekilde düzenler. */
  public update(data: UpdateRoleRequest): Observable<RoleMutationResponse> {
    return this.http.put<RoleMutationResponse>(this.url + '/' + data.id, data);
  }

  /** Sistem dışı rolün aktif/pasif durumunu değiştirir. */
  public changeStatus(data: ChangeRoleStatusRequest): Observable<RoleMutationResponse> {
    return this.http.patch<RoleMutationResponse>(
      this.url + '/' + data.id + '/status', data
    );
  }

  /** Ataması olmayan özel rolü soft delete yapar. */
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, {
      params: new HttpParams().set('version', version)
    });
  }

  /** Permission kataloğunu yalnızca yetkili kullanıcılara getirir. */
  public permissionOptions(): Observable<RolePermissionOption[]> {
    return this.http.get<RolePermissionOption[]>(this.url + '/permission-options');
  }

  /** Rolün izinlerini tek işlemle günceller. */
  public updatePermissions(data: UpdateRolePermissionsRequest):
    Observable<RoleMutationResponse> {
    return this.http.put<RoleMutationResponse>(
      this.url + '/' + data.id + '/permissions', data
    );
  }
}

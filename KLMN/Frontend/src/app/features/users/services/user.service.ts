import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import { MilitaryUnitDefOption } from '../models/military-unit-def-option.model';
import { UserAgreementPdf } from '../models/user-agreement-pdf.model';
import { ChangeUserStatusRequest } from '../models/change-user-status-request.model';
import { CreateUserRequest } from '../models/create-user-request.model';
import { UpdateUserRequest } from '../models/update-user-request.model';
import { UserListItem } from '../models/user-list-item.model';
import { UserMutationResponse } from '../models/user-mutation-response.model';
import { UserQuery } from '../models/user-query.model';

/**
 * Kullanıcı tanımları ekranının tip güvenli HTTP servisidir.
 * Angular CLI proxy olmadan environment'daki API_BASE_URL kullanılır.
 */
@Injectable({ providedIn: 'root' })
export class UserService {
  /** Angular HttpClient API çağrılarını JWT interceptor'ı ile gönderir. */
  private readonly http = inject(HttpClient);
  /** Development/Production ortamında seçilen doğrudan API adresidir. */
  private readonly apiBaseUrl = inject(API_BASE_URL);

  /**
   * Hem listeleme hem Excel aktarımı için kolon filtreleri dahil
   * sunucuya gönderilen HTTP query parametrelerini oluşturur.
   */
  private buildParams(query: UserQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);

    const textFilters = {
      search: query.search,
      fullName: query.fullName,
      userName: query.userName,
      email: query.email,
      militaryUnit: query.militaryUnit,
      role: query.role,
      validFromDate: query.validFromDate,
      validToDate: query.validToDate,
      createdDate: query.createdDate
    };

    for (const [key, value] of Object.entries(textFilters)) {
      if (value?.trim()) {
        params = params.set(key, value.trim());
      }
    }

    if (query.isActive !== null && query.isActive !== undefined)
      params = params.set('isActive', query.isActive);

    if (query.isLocked !== null && query.isLocked !== undefined)
      params = params.set('isLocked', query.isLocked);

    if (query.clockLimit !== null && query.clockLimit !== undefined)
      params = params.set('clockLimit', query.clockLimit);

    return params;
  }

  /** Tüm veriler içinde arama/kolon filtreleri uygulanmış sayfalı kullanıcı listesini getirir. */
  public getUsers(query: UserQuery): Observable<PagedResult<UserListItem>> {
    return this.http.get<PagedResult<UserListItem>>(
      `${this.apiBaseUrl}/users`, { params: this.buildParams(query) }
    );
  }

  /**
   * Seçili kolon filtrelerine uyan tüm kullanıcıları (en fazla 5000)
   * gerçek XLSX dosyası olarak backend'den alır; yalnızca mevcut sayfa değildir.
   */
  public exportUsers(query: UserQuery): Observable<Blob> {
    return this.http.get(
      `${this.apiBaseUrl}/users/export`,
      { params: this.buildParams(query), responseType: 'blob' }
    );
  }

  /** Kullanıcı formu için aktif ve silinmemiş görev yeri seçeneklerini getirir. */
  public getMilitaryUnitDefOptions(): Observable<MilitaryUnitDefOption[]> {
    return this.http.get<MilitaryUnitDefOption[]>(
      `${this.apiBaseUrl}/users/military-unit-options`
    );
  }

  /** Kullanıcının son düzenlenmiş profilini ve xmin versiyonunu okur. */
  public getUserById(id: string): Observable<UserListItem> {
    return this.http.get<UserListItem>(`${this.apiBaseUrl}/users/${id}`);
  }

  /** Kullanıcı ekler; açık parolayı kalıcı tarayıcı depolamasına yazmaz. */
  public createUser(request: CreateUserRequest): Observable<UserMutationResponse> {
    return this.http.post<UserMutationResponse>(
      `${this.apiBaseUrl}/users`, request
    );
  }

  /** Kullanıcı temel bilgilerini optimistic concurrency kontrolüyle günceller. */
  public updateUser(request: UpdateUserRequest): Observable<UserMutationResponse> {
    return this.http.put<UserMutationResponse>(
      `${this.apiBaseUrl}/users/${request.id}`, request
    );
  }

  /** Kullanıcının aktif/pasif durumunu sunucu izin kontrolüyle değiştirir. */
  public changeUserStatus(
    request: ChangeUserStatusRequest
  ): Observable<UserMutationResponse> {
    return this.http.patch<UserMutationResponse>(
      `${this.apiBaseUrl}/users/${request.id}/status`, request
    );
  }

  /** Kullanıcıya bağlı PDF metadata listesini sorgular. */
  public getAgreementPdfs(userId: string): Observable<UserAgreementPdf[]> {
    return this.http.get<UserAgreementPdf[]>(`${this.apiBaseUrl}/users/${userId}/agreements`);
  }

  /** PDF'yi multipart form ile özel sunucu klasörüne yükler. */
  public uploadAgreementPdf(userId: string, file: File): Observable<UserAgreementPdf> {
    const body = new FormData();
    body.append('file', file, file.name);
    return this.http.post<UserAgreementPdf>(`${this.apiBaseUrl}/users/${userId}/agreements`, body);
  }

  /** API yetkilendirmesi ile PDF indirme stream'i ister. */
  public downloadAgreementPdf(userId: string, documentId: string): Observable<Blob> {
    return this.http.get(`${this.apiBaseUrl}/users/${userId}/agreements/${documentId}/download`,
      { responseType: 'blob' });
  }

  /** Kullanıcının PDF belgesini soft delete eder. */
  public deleteAgreementPdf(userId: string, documentId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/users/${userId}/agreements/${documentId}`);
  }

  /** Belirtilen versiyondaki kullanıcıyı soft delete yöntemiyle siler. */
  public deleteUser(id: string, version: number): Observable<void> {
    const params = new HttpParams().set('version', version);
    return this.http.delete<void>(
      `${this.apiBaseUrl}/users/${id}`, { params }
    );
  }
}

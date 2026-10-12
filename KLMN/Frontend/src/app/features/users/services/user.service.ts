import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import { OrganizationUnitOption } from '../models/organization-unit-option.model';
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

  /** Filtrelenmiş ve sayfalanmış kullanıcı listesini sunucudan getirir. */
  public getUsers(query: UserQuery): Observable<PagedResult<UserListItem>> {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);

    const search = query.search?.trim();

    if (search) params = params.set('search', search);

    return this.http.get<PagedResult<UserListItem>>(
      `${this.apiBaseUrl}/users`, { params }
    );
  }

  /** Kullanıcı formu için aktif ve silinmemiş görev yeri seçeneklerini getirir. */
  public getOrganizationUnitOptions(): Observable<OrganizationUnitOption[]> {
    return this.http.get<OrganizationUnitOption[]>(
      `${this.apiBaseUrl}/users/organization-options`
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

import { inject, Injectable } from '@angular/core';
import {
  HttpClient,
  HttpParams
} from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import { UserListItem } from '../models/user-list-item.model';
import { UserQuery } from '../models/user-query.model';

/**
 * Kullanıcı yönetimi API servisidir.
 */
@Injectable({ providedIn: 'root' })
export class UserService {
  /** Kullanıcı listeleme ve yönetim isteklerini backend'e gönderen HttpClient'tır. */
  private readonly http = inject(HttpClient);
  /** Angular'ın istek göndereceği proxy kullanılmayan API temel adresidir. */
  private readonly apiBaseUrl = inject(API_BASE_URL);

  /** Backend'den arama metni, aktiflik ve sayfa parametrelerine göre kullanıcıları getirir. */
  public getUsers(
    query: UserQuery
  ): Observable<PagedResult<UserListItem>> {
    // Arama kriterlerini HTTP query string biçimine dönüştüren HttpParams nesnesidir.
    let params =
      new HttpParams()
        .set('pageNumber', query.pageNumber)
        .set('pageSize', query.pageSize)
        .set('includeInactive', query.includeInactive);

    // Kullanıcı listesi için uygulanacak serbest metin filtre kriteridir.
    const search = query.search?.trim();

    if (search) {
      params = params.set('search', search);
    }

    return this.http.get<PagedResult<UserListItem>>(
      `${this.apiBaseUrl}/users`,
      { params }
    );
  }
}

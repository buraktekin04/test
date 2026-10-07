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
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  public getUsers(
    query: UserQuery
  ): Observable<PagedResult<UserListItem>> {
    let params =
      new HttpParams()
        .set('pageNumber', query.pageNumber)
        .set('pageSize', query.pageSize)
        .set('includeInactive', query.includeInactive);

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

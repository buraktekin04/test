import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  CountryDefItem, CountryFields, CountryQuery,
  UpdateCountryDefRequest, ChangeCountryDefStatusRequest, CountryDefMutationResponse
} from '../models/countries.model';

@Injectable({ providedIn: 'root' })
export class CountryService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/countries';

  private params(query: CountryQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber).set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);
    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    if (query.isTownRequired !== null) params = params.set('isTownRequired', query.isTownRequired);
    return params;
  }
  public list(query: CountryQuery): Observable<PagedResult<CountryDefItem>> {
    return this.http.get<PagedResult<CountryDefItem>>(this.url, { params: this.params(query) });
  }
  public getById(id: string): Observable<CountryDefItem> {
    return this.http.get<CountryDefItem>(this.url + '/' + id);
  }
  public create(request: CountryFields): Observable<CountryDefMutationResponse> {
    return this.http.post<CountryDefMutationResponse>(this.url, request);
  }
  public update(request: UpdateCountryDefRequest): Observable<CountryDefMutationResponse> {
    return this.http.put<CountryDefMutationResponse>(this.url + '/' + request.id, request);
  }
  public changeStatus(request: ChangeCountryDefStatusRequest): Observable<CountryDefMutationResponse> {
    return this.http.patch<CountryDefMutationResponse>(this.url + '/' + request.id + '/status', request);
  }
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, {
      params: new HttpParams().set('version', version)
    });
  }
  public export(query: CountryQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', {
      params: this.params(query), responseType: 'blob'
    });
  }

}

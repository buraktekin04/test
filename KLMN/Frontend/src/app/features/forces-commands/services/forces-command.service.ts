import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  ChangeForcesCommandDefStatusRequest,
  ForcesCommandDefFields,
  ForcesCommandDefItem, ForcesCommandDefMutationResponse, ForcesCommandDefQuery,
  UpdateForcesCommandDefRequest
} from '../models/forces-command-def.model';

/** Kuvvet komutanlığı CRUD ve Excel API işlemlerinin merkezi Angular servisidir. */
@Injectable({ providedIn: 'root' })
export class ForcesCommandService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/forces-commands';

  /** Filtreleme ve Excel'in aynı query parametrelerini göndermesini sağlar. */
  private params(query: ForcesCommandDefQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize)
      .set('sortField', query.sortField)
      .set('sortOrder', query.sortOrder)
      .set('includeInactive', query.includeInactive);

    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    return params;
  }

  /** Bütün kayıtlar üzerinde filtrelenmiş sayfalı listeyi döndürür. */
  public list(query: ForcesCommandDefQuery): Observable<PagedResult<ForcesCommandDefItem>> {
    return this.http.get<PagedResult<ForcesCommandDefItem>>(this.url, {
      params: this.params(query)
    });
  }

  /** Kayıt düzenleme öncesi güncel xmin bilgisiyle detay sorgular. */
  public getById(id: string): Observable<ForcesCommandDefItem> {
    return this.http.get<ForcesCommandDefItem>(this.url + '/' + id);
  }

  /** Filtreye uygun kuvvet komutanlıklarını XLSX olarak indirir. */
  public export(query: ForcesCommandDefQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', {
      params: this.params(query), responseType: 'blob'
    });
  }

  /** Yeni kuvvet komutanlığı oluşturur. */
  public create(request: ForcesCommandDefFields):
    Observable<ForcesCommandDefMutationResponse> {
    return this.http.post<ForcesCommandDefMutationResponse>(this.url, request);
  }

  /** Kuvvet komutanlığını xmin ile güvenli şekilde günceller. */
  public update(request: UpdateForcesCommandDefRequest):
    Observable<ForcesCommandDefMutationResponse> {
    return this.http.put<ForcesCommandDefMutationResponse>(
      this.url + '/' + request.id, request
    );
  }

  /** Aktif/pasif durumunu backend ilişki kurallarıyla değiştirir. */
  public changeStatus(request: ChangeForcesCommandDefStatusRequest):
    Observable<ForcesCommandDefMutationResponse> {
    return this.http.patch<ForcesCommandDefMutationResponse>(
      this.url + '/' + request.id + '/status', request
    );
  }

  /** Bağlı birliği olmayan kuvvet komutanlığını soft delete eder. */
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, {
      params: new HttpParams().set('version', version)
    });
  }
}

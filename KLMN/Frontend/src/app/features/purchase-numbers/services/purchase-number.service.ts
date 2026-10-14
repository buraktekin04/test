import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  ChangePurchaseNumberDefStatusRequest,
  PurchaseNumberDefFields,
  PurchaseNumberDefItem, PurchaseNumberDefMutationResponse, PurchaseNumberDefQuery,
  UpdatePurchaseNumberDefRequest
} from '../models/purchase-number-def.model';

/** Alım numarası CRUD ve Excel API işlemlerinin merkezi Angular servisidir. */
@Injectable({ providedIn: 'root' })
export class PurchaseNumberService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/purchase-numbers';

  /** Filtreleme ve Excel'in aynı query parametrelerini göndermesini sağlar. */
  private params(query: PurchaseNumberDefQuery): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize)
      .set('includeInactive', query.includeInactive);

    if (query.search.trim()) params = params.set('search', query.search.trim());
    if (query.isActive !== null) params = params.set('isActive', query.isActive);
    if (query.supplyDescription.trim())
      params = params.set('supplyDescription', query.supplyDescription.trim());
    return params;
  }

  /** Bütün kayıtlar üzerinde filtrelenmiş sayfalı listeyi döndürür. */
  public list(query: PurchaseNumberDefQuery): Observable<PagedResult<PurchaseNumberDefItem>> {
    return this.http.get<PagedResult<PurchaseNumberDefItem>>(this.url, {
      params: this.params(query)
    });
  }

  /** Kayıt düzenleme öncesi güncel xmin bilgisiyle detay sorgular. */
  public getById(id: string): Observable<PurchaseNumberDefItem> {
    return this.http.get<PurchaseNumberDefItem>(this.url + '/' + id);
  }

  /** Filtreye uygun kuvvet komutanlıklarını XLSX olarak indirir. */
  public export(query: PurchaseNumberDefQuery): Observable<Blob> {
    return this.http.get(this.url + '/export', {
      params: this.params(query), responseType: 'blob'
    });
  }

  /** Yeni alım numarası oluşturur. */
  public create(request: PurchaseNumberDefFields):
    Observable<PurchaseNumberDefMutationResponse> {
    return this.http.post<PurchaseNumberDefMutationResponse>(this.url, request);
  }

  /** Alım numarasını xmin ile güvenli şekilde günceller. */
  public update(request: UpdatePurchaseNumberDefRequest):
    Observable<PurchaseNumberDefMutationResponse> {
    return this.http.put<PurchaseNumberDefMutationResponse>(
      this.url + '/' + request.id, request
    );
  }

  /** Aktif/pasif durumunu backend ilişki kurallarıyla değiştirir. */
  public changeStatus(request: ChangePurchaseNumberDefStatusRequest):
    Observable<PurchaseNumberDefMutationResponse> {
    return this.http.patch<PurchaseNumberDefMutationResponse>(
      this.url + '/' + request.id + '/status', request
    );
  }

  /** Bağlı birliği olmayan alım numarasını soft delete eder. */
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + id, {
      params: new HttpParams().set('version', version)
    });
  }
}

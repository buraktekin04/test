import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../config/api.tokens';
import { PagedResult } from '../models/paged-result.model';
import { AuditLogItem, AuditLogQuery, AuditFieldChange } from './audit-log.model';

/** Kullanıcı, rol ve diğer tanımlamaların ortak, salt okunur geçmiş servisidir. */
@Injectable({ providedIn: 'root' })
export class AuditLogService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/audit-logs';

  /** Sunucuda sayfalanmış audit geçmişini güvenli parametrelerle sorgular. */
  public list(query: AuditLogQuery): Observable<PagedResult<AuditLogItem>> {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize);
    for (const key of ['entityType', 'entityId', 'action', 'actorUserId', 'from', 'to'] as const) {
      const value = query[key];
      if (value) params = params.set(key, value);
    }
    return this.http.get<PagedResult<AuditLogItem>>(this.url, { params });
  }

  /** Yalnızca sunucunun izinli değişiklik JSON'unu okunabilir alan satırlarına dönüştürür. */
  public changes(item: AuditLogItem): Array<{ field: string; oldValue: string; newValue: string }> {
    try {
      const data = JSON.parse(item.changesJson) as Record<string, AuditFieldChange>;
      if (!data || typeof data !== 'object' || Array.isArray(data)) return [];
      return Object.entries(data).map(([field, values]) => ({
        field: this.fieldLabel(field),
        oldValue: this.format(values?.oldValue),
        newValue: this.format(values?.newValue)
      }));
    } catch { return []; }
  }

  /** Tanımlama ekranlarında teknik EF alanlarını anlaşılır Türkçe başlıklara dönüştürür. */
  private fieldLabel(field: string): string {
    const labels: Record<string, string> = {
      UserName: 'Kullanıcı Adı', FirstName: 'Ad', LastName: 'Soyad',
      Email: 'E-posta', PhoneNumber: 'Telefon',
      OrganizationUnitId: 'Görev Yeri', RoleId: 'Rol',
      UserId: 'Kullanıcı', PermissionId: 'Yetki',
      IsGranted: 'Yetki Verildi', Name: 'Ad', Code: 'Kod',
      Description: 'Açıklama', Module: 'Modül',
      IsActive: 'Aktiflik', ClockLimit: 'Mesai Dışı Kısıtı',
      IsLocked: 'Kilit Durumu', ValidFrom: 'Geçerlilik Başlangıcı',
      ValidTo: 'Geçerlilik Bitişi', OriginalFileName: 'PDF Adı',
      SizeBytes: 'Dosya Boyutu', SortOrder: 'Sıralama'
    };
    return labels[field] ?? field;
  }

  /** Boş, tarih ve boolean gibi değerleri arayüzde uygun metinle gösterir. */
  private format(value: unknown): string {
    if (value === null || value === undefined) return '—';
    if (typeof value === 'boolean') return value ? 'Evet' : 'Hayır';
    if (typeof value === 'object') return JSON.stringify(value);
    return String(value);
  }
}

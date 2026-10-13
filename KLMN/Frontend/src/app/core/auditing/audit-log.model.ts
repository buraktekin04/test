/** Değiştirilemez audit geçmişindeki izinli alanın eski ve yeni değeridir. */
export interface AuditFieldChange {
  oldValue: unknown;
  newValue: unknown;
}

/** API'den dönen geçmiş kaydı ve JSON içindeki alan farklarıdır. */
export interface AuditLogItem {
  id: string;
  occurredAt: string;
  actorUserId: string | null;
  actorUserName: string | null;
  entityType: string;
  entityId: string;
  action: string;
  changesJson: string;
  ipAddress: string | null;
}

/** Genel geçmişin filtrelenmiş sorgusu veya tek kayıt geçmişidir. */
export interface AuditLogQuery {
  entityType?: string;
  entityId?: string;
  action?: string;
  actorUserId?: string;
  from?: string;
  to?: string;
  pageNumber: number;
  pageSize: number;
}

/** Bağlı Kurul Kararı DTO'su. Audit/xmin BaseEntity, tarihler EffectiveDatedEntity üzerinden gelir. */
export interface ConnectedDecisionDefItem {
  id: string;
  code: string;
  name: string;
  isActive: boolean;
  validFrom: string | null;
  validTo: string | null;
  createdDate: string;
  createdBy: string | null;
  updatedDate: string | null;
  updatedBy: string | null;
  version: number;
}

/** Create ve Update işlemlerinin iş alanları. */
export interface ConnectedDecisionFields {
  code: string;
  name: string;
  validFrom: string | null;
  validTo: string | null;
}

/** Sorgu sayfalama ve BaseEntity.IsActive alanını filtreler. */
export interface ConnectedDecisionQuery {
  search: string;
  includeInactive: boolean;
  isActive: boolean | null;
  pageNumber: number;
  pageSize: number;
}
export interface UpdateConnectedDecisionDefRequest extends ConnectedDecisionFields { id: string; version: number; }
export interface ChangeConnectedDecisionDefStatusRequest { id: string; version: number; isActive: boolean; }
export interface ConnectedDecisionDefMutationResponse { id: string; version: number; }

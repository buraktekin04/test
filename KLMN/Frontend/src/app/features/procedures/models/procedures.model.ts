/** Tetkik DTO'su. Audit/xmin BaseEntity, tarihler EffectiveDatedEntity üzerinden gelir. */
export interface ProcedureDefItem {
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
export interface ProcedureFields {
  code: string;
  name: string;
  validFrom: string | null;
  validTo: string | null;
}

/** Sorgu sayfalama ve BaseEntity.IsActive alanını filtreler. */
export interface ProcedureQuery {
  search: string;
  includeInactive: boolean;
  isActive: boolean | null;
  pageNumber: number;
  pageSize: number;
}
export interface UpdateProcedureDefRequest extends ProcedureFields { id: string; version: number; }
export interface ChangeProcedureDefStatusRequest { id: string; version: number; isActive: boolean; }
export interface ProcedureDefMutationResponse { id: string; version: number; }

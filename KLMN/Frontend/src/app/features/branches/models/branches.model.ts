/** Branş DTO'su. Audit/xmin BaseEntity, tarihler EffectiveDatedEntity üzerinden gelir. */
export interface BranchDefItem {
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
export interface BranchFields {
  code: string;
  name: string;
  validFrom: string | null;
  validTo: string | null;
}

/** Sorgu sayfalama ve BaseEntity.IsActive alanını filtreler. */
export interface BranchQuery {
  search: string;
  includeInactive: boolean;
  isActive: boolean | null;
  pageNumber: number;
  pageSize: number;
}
export interface UpdateBranchDefRequest extends BranchFields { id: string; version: number; }
export interface ChangeBranchDefStatusRequest { id: string; version: number; isActive: boolean; }
export interface BranchDefMutationResponse { id: string; version: number; }

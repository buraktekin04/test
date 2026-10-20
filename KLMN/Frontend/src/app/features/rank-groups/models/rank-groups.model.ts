/** Rütbe Grubu DTO'su. Audit/xmin BaseEntity, tarihler EffectiveDatedEntity üzerinden gelir. */
export interface RankGroupDefItem {
  id: string;
  code: string;
  name: string;
  shortName: string | null;
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
export interface RankGroupFields {
  code: string;
  name: string;
  shortName: string | null;
  validFrom: string | null;
  validTo: string | null;
}

/** Sorgu sayfalama ve BaseEntity.IsActive alanını filtreler. */
export interface RankGroupQuery {
  search: string;
  includeInactive: boolean;
  isActive: boolean | null;
  pageNumber: number;
  pageSize: number;
}
export interface UpdateRankGroupDefRequest extends RankGroupFields { id: string; version: number; }
export interface ChangeRankGroupDefStatusRequest { id: string; version: number; isActive: boolean; }
export interface RankGroupDefMutationResponse { id: string; version: number; }

/** Rütbe DTO'su. Audit/xmin BaseEntity, tarihler EffectiveDatedEntity üzerinden gelir. */
export interface RankDefItem {
  id: string;
  code: string;
  name: string;
  shortName: string | null;
  type: number;
  rankGroupId: string | null;
  rankGroupName: string | null;
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
export interface RankFields {
  code: string;
  name: string;
  shortName: string | null;
  type: number;
  rankGroupId: string | null;
  validFrom: string | null;
  validTo: string | null;
}

/** Sorgu sayfalama ve BaseEntity.IsActive alanını filtreler. */
export interface RankQuery {
  search: string;
  includeInactive: boolean;
  isActive: boolean | null;
  pageNumber: number;
  pageSize: number;
}
export interface UpdateRankDefRequest extends RankFields { id: string; version: number; }
export interface ChangeRankDefStatusRequest { id: string; version: number; isActive: boolean; }
export interface RankDefMutationResponse { id: string; version: number; }

/** Rütbe formunun aktif rütbe grubu seçenekleri. */
export interface RankOption { id: string; code: string; name: string; }

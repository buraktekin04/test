/** Askerî Sınıf DTO'su. Audit/xmin BaseEntity, tarihler EffectiveDatedEntity üzerinden gelir. */
export interface MilitaryClassDefItem {
  id: string;
  code: string;
  name: string;
  shortName: string | null;
  externalCode: string | null;
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
export interface MilitaryClassFields {
  code: string;
  name: string;
  shortName: string | null;
  externalCode: string | null;
  validFrom: string | null;
  validTo: string | null;
}

/** Sorgu sayfalama ve BaseEntity.IsActive alanını filtreler. */
export interface MilitaryClassQuery {
  search: string;
  includeInactive: boolean;
  isActive: boolean | null;
  pageNumber: number;
  pageSize: number;
}
export interface UpdateMilitaryClassDefRequest extends MilitaryClassFields { id: string; version: number; }
export interface ChangeMilitaryClassDefStatusRequest { id: string; version: number; isActive: boolean; }
export interface MilitaryClassDefMutationResponse { id: string; version: number; }

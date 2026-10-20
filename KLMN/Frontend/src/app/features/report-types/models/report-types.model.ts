/** Rapor Türü DTO'su. Audit/xmin BaseEntity, tarihler EffectiveDatedEntity üzerinden gelir. */
export interface ReportTypeDefItem {
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
export interface ReportTypeFields {
  code: string;
  name: string;
  validFrom: string | null;
  validTo: string | null;
}

/** Sorgu sayfalama ve BaseEntity.IsActive alanını filtreler. */
export interface ReportTypeQuery {
  search: string;
  includeInactive: boolean;
  isActive: boolean | null;
  pageNumber: number;
  pageSize: number;
}
export interface UpdateReportTypeDefRequest extends ReportTypeFields { id: string; version: number; }
export interface ChangeReportTypeDefStatusRequest { id: string; version: number; isActive: boolean; }
export interface ReportTypeDefMutationResponse { id: string; version: number; }

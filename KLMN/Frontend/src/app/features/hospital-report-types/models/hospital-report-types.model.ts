/** Sağlık Tesisi Rapor Türü DTO'su. Audit, aktiflik ve xmin BaseEntity üzerinden gelir. */
export interface HospitalReportTypeItem {
  id: string;
  hospitalDefId: string;
  hospitalName: string;
  reportTypeDefId: string;
  reportTypeName: string;
  isActive: boolean;
  createdDate: string;
  createdBy: string | null;
  updatedDate: string | null;
  updatedBy: string | null;
  version: number;
}

/** Create ve Update işlemlerinin iş alanları. */
export interface HospitalReportTypeFields {
  hospitalDefId: string;
  reportTypeDefId: string;
}

/** Sorgu sayfalama ve BaseEntity.IsActive alanını filtreler. */
export interface HospitalReportTypeQuery {
  search: string;
  includeInactive: boolean;
  isActive: boolean | null;
  pageNumber: number;
  pageSize: number;
}
export interface UpdateHospitalReportTypeRequest extends HospitalReportTypeFields { id: string; version: number; }
export interface ChangeHospitalReportTypeStatusRequest { id: string; version: number; isActive: boolean; }
export interface HospitalReportTypeMutationResponse { id: string; version: number; }

/** Dropdown option ve seçenek grupları. */
export interface DefinitionOption { id: string; code: string; name: string; }
export interface HospitalReportTypeOptions { hospitals: DefinitionOption[]; reportTypes: DefinitionOption[]; }

/** Sağlık Tesisi DTO'su. Audit/xmin BaseEntity, tarihler EffectiveDatedEntity üzerinden gelir. */
export interface HospitalDefItem {
  id: string;
  code: string;
  name: string;
  cityId: string | null;
  cityName: string | null;
  refereeHospitalId: string | null;
  refereeHospitalName: string | null;
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
export interface HospitalFields {
  code: string;
  name: string;
  cityId: string | null;
  refereeHospitalId: string | null;
  validFrom: string | null;
  validTo: string | null;
}

/** Sorgu sayfalama ve BaseEntity.IsActive alanını filtreler. */
export interface HospitalQuery {
  search: string;
  includeInactive: boolean;
  isActive: boolean | null;
  pageNumber: number;
  pageSize: number;
}
export interface UpdateHospitalDefRequest extends HospitalFields { id: string; version: number; }
export interface ChangeHospitalDefStatusRequest { id: string; version: number; isActive: boolean; }
export interface HospitalDefMutationResponse { id: string; version: number; }

/** Şehir ve hakem sağlık tesisi seçenekleri. */
export interface HospitalOption { id: string; code: string; name: string; }
export interface HospitalOptions { cities: HospitalOption[]; refereeHospitals: HospitalOption[]; }

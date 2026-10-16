/** API'deki MilitaryOfficeDef; CityDef dışında ek ilişki içermez. */
export interface MilitaryOfficeDefItem {
  id: string;
  code: string;
  name: string;
  externalCode: string | null;
  cityId: string | null;
  cityName: string | null;
  validFrom: string | null;
  validTo: string | null;
  isActive: boolean;
  createdDate: string;
  createdBy: string | null;
  updatedDate: string | null;
  updatedBy: string | null;
  version: number;
}

/** Kayıt ekleme/düzenleme alanları. */
export interface MilitaryOfficeFields {
  code: string;
  name: string;
  externalCode: string | null;
  cityId: string | null;
  validFrom: string | null;
  validTo: string | null;
}

/** Liste, kolon filtreleri ve Excel'in ortak sorgu parametreleri. */
export interface MilitaryOfficeQuery {
  search: string;
  includeInactive: boolean;
  isActive: boolean | null;
  cityId: string | null;
  pageNumber: number;
  pageSize: number;
}
export interface UpdateMilitaryOfficeDefRequest extends MilitaryOfficeFields { id: string; version: number; }
export interface ChangeMilitaryOfficeDefStatusRequest { id: string; version: number; isActive: boolean; }
export interface MilitaryOfficeDefMutationResponse { id: string; version: number; }
/** Şehir seçenekleri id/kod/ad alanlarını içerir. */
export interface MilitaryOfficeOption { id: string; code: string; name: string; }

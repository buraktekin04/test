/** Backend TownDef alanları; audit ve xmin BaseEntity'den gelir. */
export interface TownDefItem {
  id: string;
  code: string;
  name: string;
  externalCode: string | null;
  /** Eski TownDef.CityDef alanının nullable FK karşılığıdır. */
  cityId: string | null;
  cityName: string | null;
  mernisCode: number | null;
  isActive: boolean;
  createdDate: string;
  createdBy: string | null;
  updatedDate: string | null;
  updatedBy: string | null;
  version: number;
}
/** Oluşturma ve güncellemede sunucuya gönderilen ilçe alanları. */
export interface TownFields {
  code: string; name: string; externalCode: string | null;
  cityId: string | null; mernisCode: number | null;
}
/** Backend filtreleme ve sayfalama sözleşmesi. */
export interface TownQuery {
  search: string; includeInactive: boolean; isActive: boolean | null;
  cityId: string | null;
  pageNumber: number; pageSize: number;
}
export interface UpdateTownDefRequest extends TownFields { id: string; version: number; }
export interface ChangeTownDefStatusRequest { id: string; version: number; isActive: boolean; }
export interface TownDefMutationResponse { id: string; version: number; }
export interface TownCityOption { id: string; code: string; name: string; }

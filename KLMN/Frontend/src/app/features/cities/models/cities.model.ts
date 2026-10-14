/** Backend CityDef alanları; audit ve xmin BaseEntity'den gelir. */
export interface CityDefItem {
  id: string;
  code: string;
  name: string;
  externalCode: string | null;
    countryId: string | null;
  countryName: string | null;
  mernisCode: number | null;
  validFrom: string | null;
  validTo: string | null;
  isActive: boolean;
  createdDate: string;
  createdBy: string | null;
  updatedDate: string | null;
  updatedBy: string | null;
  version: number;
}
export interface CityFields {
  code: string; name: string; externalCode: string | null;
  countryId: string | null; mernisCode: number | null;
  validFrom: string | null; validTo: string | null;
}
export interface CityQuery {
  search: string; includeInactive: boolean; isActive: boolean | null;
  countryId: string | null;
  pageNumber: number; pageSize: number;
}
export interface UpdateCityDefRequest extends CityFields { id: string; version: number; }
export interface ChangeCityDefStatusRequest { id: string; version: number; isActive: boolean; }
export interface CityDefMutationResponse { id: string; version: number; }
export interface CityCountryOption { id: string; code: string; name: string; }

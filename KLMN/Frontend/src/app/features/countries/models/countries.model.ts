/** Backend CountryDef alanları; audit ve xmin BaseEntity'den gelir. */
export interface CountryDefItem {
  id: string;
  code: string;
  name: string;
  externalCode: string | null;
    isTownRequired: boolean | null;
  cityCount: number;
  isActive: boolean;
  createdDate: string;
  createdBy: string | null;
  updatedDate: string | null;
  updatedBy: string | null;
  version: number;
}
export interface CountryFields {
  code: string; name: string; externalCode: string | null;
  isTownRequired: boolean | null;
}
export interface CountryQuery {
  search: string; includeInactive: boolean; isActive: boolean | null;
  isTownRequired: boolean | null;
  pageNumber: number; pageSize: number;
}
export interface UpdateCountryDefRequest extends CountryFields { id: string; version: number; }
export interface ChangeCountryDefStatusRequest { id: string; version: number; isActive: boolean; }
export interface CountryDefMutationResponse { id: string; version: number; }

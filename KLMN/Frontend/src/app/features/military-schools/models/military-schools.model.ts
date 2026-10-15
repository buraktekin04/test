/** API'deki MilitarySchoolDef DTO'su; audit ve xmin BaseEntity'den gelir. */
export interface MilitarySchoolDefItem {
  id: string;
  code: string;
  name: string;
  cityId: string | null;
  cityName: string | null;
  forcesCommandDefId: string | null;
  forcesCommandName: string | null;
  validFrom: string | null;
  validTo: string | null;
  isActive: boolean;
  createdDate: string;
  createdBy: string | null;
  updatedDate: string | null;
  updatedBy: string | null;
  version: number;
}

/** Kayıt oluşturma/düzenlemede gönderilen okul alanları. */
export interface MilitarySchoolFields {
  code: string; name: string;
  cityId: string | null;
  forcesCommandDefId: string | null;
  validFrom: string | null;
  validTo: string | null;
}

/** Backend'de çalışan arama, durum, referans ve sayfalama parametreleri. */
export interface MilitarySchoolQuery {
  search: string; includeInactive: boolean;
  isActive: boolean | null;
  cityId: string | null;
  forcesCommandDefId: string | null;
  pageNumber: number; pageSize: number;
}
export interface UpdateMilitarySchoolDefRequest extends MilitarySchoolFields { id: string; version: number; }
export interface ChangeMilitarySchoolDefStatusRequest { id: string; version: number; isActive: boolean; }
export interface MilitarySchoolDefMutationResponse { id: string; version: number; }
/** Dropdown'da okunabilen temel güvenli şehir/kuvvet bilgileri. */
export interface MilitarySchoolOption { id: string; code: string; name: string; }
export interface MilitarySchoolOptions {
  cities: MilitarySchoolOption[];
  forcesCommands: MilitarySchoolOption[];
}

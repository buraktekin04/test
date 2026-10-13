/** Birlik Tanımları listesinin ve detay endpoint'inin yanıtıdır. */
export interface MilitaryUnitDefItem {
  externalCode: string | null;
  forcesCommandDef: string | null;
  forcesCommandName: string | null;
  forcesCommandCode: string | null;
  validFrom: string | null;
  validTo: string | null;
  id: string;
  code: string;
  name: string;
  connectedTopUnity: string | null;
  connectedTopUnitName: string | null;
  connectedTopUnitCode: string | null;
  isActive: boolean;
  createdDate: string;
  version: number;
  userCount: number;
  childCount: number;
}

/** Üst birlik seçiciye getirilen aktif birlik kaydıdır. */
export interface MilitaryUnitParentOption {
  id: string;
  code: string;
  name: string;
  connectedTopUnity: string | null;
}

/** Liste sorgusu ile Excel aktarımındaki ortak filtre modelidir. */
export interface MilitaryUnitDefQuery {
  search: string;
  includeInactive: boolean;
  isActive: boolean | null;
  rootOnly: boolean | null;
  pageNumber: number;
  pageSize: number;
}

/** Birlik temel bilgilerini içeren oluşturma isteğidir. */
export interface MilitaryUnitDefFields {
  externalCode: string | null;
  forcesCommandDef: string | null;
  validFrom: string | null;
  validTo: string | null;
  name: string;
  code: string;
  connectedTopUnity: string | null;
}

/** Güncelleme isteğinde optimistic concurrency için xmin bilgisi taşınır. */
export interface UpdateMilitaryUnitDefRequest extends MilitaryUnitDefFields {
  id: string;
  version: number;
}

/** Aktiflik değişiminde xmin bilgisiyle sunucu koruması kullanılır. */
export interface ChangeMilitaryUnitDefStatusRequest {
  id: string;
  version: number;
  isActive: boolean;
}

/** Oluşturma/güncelleme sonrasında dönen yeni xmin değeridir. */
export interface MilitaryUnitDefMutationResponse {
  id: string;
  version: number;
}

/** Birlik seçimine sunulan aktif/geçerli kuvvet komutanlığı. */
export interface ForceCommandOption { id: string; code: string; name: string; }

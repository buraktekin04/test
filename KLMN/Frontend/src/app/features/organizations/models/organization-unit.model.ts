/** Organizasyon Tanımları listesinin ve detay endpoint'inin yanıtıdır. */
export interface OrganizationUnitItem {
  id: string;
  code: string;
  name: string;
  parentOrganizationUnitId: string | null;
  parentOrganizationUnitName: string | null;
  parentOrganizationUnitCode: string | null;
  isActive: boolean;
  createdDate: string;
  version: number;
  userCount: number;
  childCount: number;
}

/** Üst organizasyon seçiciye getirilen aktif birim kaydıdır. */
export interface OrganizationParentOption {
  id: string;
  code: string;
  name: string;
  parentOrganizationUnitId: string | null;
}

/** Liste sorgusu ile Excel aktarımındaki ortak filtre modelidir. */
export interface OrganizationUnitQuery {
  search: string;
  includeInactive: boolean;
  isActive: boolean | null;
  rootOnly: boolean | null;
  pageNumber: number;
  pageSize: number;
}

/** Birim temel bilgilerini içeren oluşturma isteğidir. */
export interface OrganizationUnitFields {
  name: string;
  code: string;
  parentOrganizationUnitId: string | null;
}

/** Güncelleme isteğinde optimistic concurrency için xmin bilgisi taşınır. */
export interface UpdateOrganizationUnitRequest extends OrganizationUnitFields {
  id: string;
  version: number;
}

/** Aktiflik değişiminde xmin bilgisiyle sunucu koruması kullanılır. */
export interface ChangeOrganizationUnitStatusRequest {
  id: string;
  version: number;
  isActive: boolean;
}

/** Oluşturma/güncelleme sonrasında dönen yeni xmin değeridir. */
export interface OrganizationUnitMutationResponse {
  id: string;
  version: number;
}

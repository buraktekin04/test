/** Rol listesi, detay ve izin panelinin API yanıtıdır. */
export interface RoleItem {
  /** Rolün PostgreSQL üzerindeki benzersiz anahtarıdır. */
  id: string;
  /** Rolün kullanıcıya gösterilen adıdır. */
  name: string;
  /** Rolün yetkilendirme için kullanılan teknik kodudur. */
  code: string;
  /** İsteğe bağlı rol açıklamasıdır. */
  description: string | null;
  /** ADMIN/STANDARD_USER gibi korunan rollerin işaretidir. */
  isSystemRole: boolean;
  /** Rolün uygulamada kullanılabilir olup olmadığını gösterir. */
  isActive: boolean;
  /** Kaydın UTC oluşturulma tarihidir. */
  createdDate: string;
  /** PostgreSQL xmin optimistic concurrency değeridir. */
  version: number;
  /** Bu rolü kullanan kullanıcı sayısıdır. */
  userCount: number;
  /** Role atanmış etkin izinlerin teknik kodlarıdır. */
  permissionCodes: string[];
}

/** Rol listeleme ve Excel'in ortak arama/sayfalama parametreleridir. */
export interface RoleQuery {
  search: string;
  includeInactive: boolean;
  pageNumber: number;
  pageSize: number;
  isActive: boolean | null;
  isSystemRole: boolean | null;
}

/** Role ait yeni ad, kod ve açıklama bilgisidir. */
export interface RoleFields {
  name: string;
  code: string;
  description: string | null;
}

/** Rol düzenleme işlemine eklenen kimlik ve concurrency alanlarıdır. */
export interface UpdateRoleRequest extends RoleFields {
  id: string;
  version: number;
}

/** Rol aktifliğini güncellemek için kullanılan komutun JSON karşılığıdır. */
export interface ChangeRoleStatusRequest {
  id: string;
  version: number;
  isActive: boolean;
}

/** Permission seçiciye gelen sabit katalog seçeneğidir. */
export interface RolePermissionOption {
  id: string;
  code: string;
  name: string;
  module: string;
  description: string | null;
  sortOrder: number;
}

/** Rol yetki güncelleme JSON sözleşmesidir. */
export interface UpdateRolePermissionsRequest {
  id: string;
  version: number;
  permissionIds: string[];
}

/** Güncelleme sonrasında EF Core xmin versiyonunu içeren yanıttır. */
export interface RoleMutationResponse {
  id: string;
  version: number;
}

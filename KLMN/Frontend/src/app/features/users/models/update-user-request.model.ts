/**
 * Kullanıcı profilini parola/rol değiştirmeden güncelleme isteğidir.
 * PostgreSQL xmin versiyonu değişen kayıtları korur.
 */
export interface UpdateUserRequest {
  /** Güncellenecek kullanıcının GUID'i. */
  id: string;
  /** İstemcinin son okuduğu satır versiyonu. */
  version: number;
  /** Benzersiz kullanıcı adı. */
  userName: string;
  /** Ad. */
  firstName: string;
  /** Soyad. */
  lastName: string;
  /** E-posta. */
  email: string;
  /** Telefon. */
  phoneNumber: string | null;
  /**
   * Kullanıcının görev yeri olarak atanan organizasyon biriminin GUID'idir.
   * İsteğe bağlıdır ve herhangi bir rol/permission ataması yapmaz.
   */
  organizationUnitId: string | null;
  /** true: mesai dışı erişim kısıtlıdır; false: mesai dışı izinlidir. */
  clockLimit: boolean;
  /** Hesabın manuel olarak kilitlenip kilitlenmediği; mevcut alan güncellenir. */
  isLocked: boolean;

  /** Hesap geçerlilik başlangıcı UTC ISO-8601 (dahil; null sınırsız). */
  validFrom: string | null;

  /** Hesap geçerlilik bitişi UTC ISO-8601 (hariç; null sınırsız). */
  validTo: string | null;
}

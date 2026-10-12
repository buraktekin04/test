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
}

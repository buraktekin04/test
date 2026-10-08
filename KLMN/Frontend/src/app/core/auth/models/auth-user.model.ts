/**
 * Oturum açmış kullanıcıya ait profil ve UI yetki bilgilerini temsil eder.
 */
export interface AuthUser {
  /** Kullanıcının veritabanındaki benzersiz GUID kimliğidir. */
  id: string;
  /** Hesap girişinde kullanılan kullanıcı adıdır. */
  userName: string;
  /** Kullanıcının adıdır. */
  firstName: string;
  /** Kullanıcının soyadıdır. */
  lastName: string;
  /** Ad ve soyadın arayüz için birleştirilmiş biçimidir. */
  fullName: string;
  /** Kullanıcının e-posta adresidir. */
  email: string;
  /** Kullanıcının isteğe bağlı telefon numarasıdır. */
  phoneNumber?: string | null;
  /** Kullanıcının bağlı olduğu birimin benzersiz GUID değeridir. */
  organizationUnitId: string | null;
  /** Kullanıcının bağlı olduğu birimin görüntülenebilir adıdır. */
  organizationUnitName?: string | null;
  /** Kullanıcının son başarılı oturum açma UTC zamanıdır. */
  lastLoginDate?: string | null;
  /** Parolanın en son değiştirildiği UTC zamandır. */
  passwordChangedDate?: string | null;
  /** Kullanıcıya atanmış etkin rol kodlarıdır. */
  roles: string[];
  /** Rol ve kullanıcı override'larından hesaplanan izin kodlarıdır. */
  permissions: string[];
}

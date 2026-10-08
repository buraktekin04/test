/**
 * Kullanıcı listeleme özet modelidir.
 */
export interface UserListItem {
  /** Kullanıcı veya navigasyon öğesinin benzersiz tanımlayıcısıdır. */
  id: string;
  /** Oturum açan kullanıcının sistemdeki hesap adıdır. */
  userName: string;
  /** Kullanıcının adıdır. */
  firstName: string;
  /** Kullanıcının soyadıdır. */
  lastName: string;
  /** Ad ve soyadın arayüzde gösterilen tam biçimidir. */
  fullName: string;
  /** Kullanıcıya ait zorunlu e-posta adresidir. */
  email: string;
  /** Kullanıcının isteğe bağlı telefon numarasıdır. */
  phoneNumber: string | null;
  /** Kullanıcının bağlı olduğu organizasyon biriminin GUID kimliğidir. */
  organizationUnitId: string | null;
  /** Organizasyon biriminin uygulama içinde kullanılan teknik kodudur. */
  organizationUnitCode: string | null;
  /** Bağlı olduğu kurumsal birimin kullanıcıya gösterilen adıdır. */
  organizationUnitName: string | null;
  /** Kullanıcının aktif rol kodlarını içerir. */
  roles: string[];
  /** Kaydın iş süreçlerinde aktif olup olmadığını gösterir. */
  isActive: boolean;
  /** Kullanıcı hesabının erişim kilidi durumudur. */
  isLocked: boolean;
  /** Kaydın UTC oluşturulma zamanıdır. */
  createdDate: string;
  /** PostgreSQL xmin'den gelen optimistic concurrency kontrol değeridir. */
  version: number;
}

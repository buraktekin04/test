/**
 * Kullanıcı sorgu modelidir.
 */
export interface UserQuery {
  /** Kullanıcı listesi için uygulanacak serbest metin filtre kriteridir. */
  search?: string;
  /** Pasif kullanıcıların da sorguya katılıp katılmayacağını belirler. */
  includeInactive: boolean;
  /** Bir tabanlı mevcut sayfa numarasıdır. */
  pageNumber: number;
  /** Sayfa başına alınacak kullanıcı kayıt sayısıdır. */
  pageSize: number;
}

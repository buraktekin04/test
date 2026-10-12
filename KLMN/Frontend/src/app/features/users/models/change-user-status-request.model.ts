/** Kullanıcı hesabını aktif/pasif yapma isteğidir. */
export interface ChangeUserStatusRequest {
  /** İşlem yapılacak kullanıcı GUID'i. */
  id: string;
  /** İşlem öncesi PostgreSQL xmin değeri. */
  version: number;
  /** İşlem sonunda istenen aktiflik durumu. */
  isActive: boolean;
}

/** Oluşturma, güncelleme veya durum değiştirme sonrası API'nin dönüşüdür. */
export interface UserMutationResponse {
  /** İşlem yapılan kullanıcı GUID'i. */
  id: string;
  /** Başarılı işlem sonrasındaki güncel PostgreSQL xmin versiyonu. */
  version: number;
}

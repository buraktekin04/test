/** Kullanıcıya ait PDF meta verileri; fiziksel disk yolu ve byte verisi dahil edilmez. */
export interface UserAgreementPdf {
  /** Belgenin benzersiz kimliği. */
  id: string;
  /** Kullanıcı kimliği. */
  userId: string;
  /** Orijinal PDF dosya adı. */
  fileName: string;
  /** PDF boyutu (byte). */
  sizeBytes: number;
  /** Oluşturulma zamanı UTC. */
  createdDate: string;
}

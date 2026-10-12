/**
 * Kullanıcı ve ilerideki tanım ekranlarının ortak geçerlilik aralığı DTO'su.
 */
export interface ValidityPeriod {
  /** UTC başlangıç (dahil); null geçmiş sınırı yok demektir. */
  validFrom: string | null;
  /** UTC bitiş (hariç); null gelecek sınırı yok demektir. */
  validTo: string | null;
}

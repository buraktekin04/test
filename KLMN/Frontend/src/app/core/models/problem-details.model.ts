/**
 * Backend ProblemDetails hata modelidir.
 */
export interface ProblemDetails {
  /** Standart ProblemDetails hata başlığıdır. */
  title?: string;
  /** HTTP cevabının durum kodudur. */
  status?: number;
  /** Backend tarafından döndürülen güvenli hata açıklamasıdır. */
  detail?: string;
  /** instance alanını API ile paylaşılan tip güvenli veri modelinde taşır. */
  instance?: string;
  [key: string]: unknown;
}

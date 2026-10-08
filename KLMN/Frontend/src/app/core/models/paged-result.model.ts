/**
 * Standart sayfalı response modelidir.
 */
export interface PagedResult<T> {
  /** API'den gelen ilgili sayfanın kayıt koleksiyonudur. */
  items: T[];
  /** Filtreye uyan toplam kayıt sayısıdır. */
  totalCount: number;
  /** Bir tabanlı sayfa numarasıdır. */
  pageNumber: number;
  /** Tek sayfada gösterilecek kayıt sayısıdır. */
  pageSize: number;
  /** Toplam kayıt sayısından hesaplanan sayfa adedidir. */
  totalPages: number;
}

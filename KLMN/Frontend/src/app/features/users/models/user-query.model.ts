/**
 * Kullanıcı tablosunun sunucu taraflı genel arama, kolon filtresi ve
 * sayfalama kriterleridir. Excel aktarımı da bu kriterleri kullanır.
 */
export interface UserQuery {
  sortField: string;
  sortOrder: number;
  /** Kullanıcılarda serbest metin aramasıdır. */
  search?: string;
  /** Pasif kullanıcıları da arama kapsamına alır. */
  includeInactive: boolean;
  /** 1 tabanlı sayfa numarasıdır. */
  pageNumber: number;
  /** Sayfa başına kayıt sayısıdır. */
  pageSize: number;
  /** Ad ve soyad filtresidir. */
  fullName?: string;
  /** Kullanıcı adı filtresidir. */
  userName?: string;
  /** E-posta filtresidir. */
  email?: string;
  /** Birlik kodu veya adı filtresidir. */
  militaryUnit?: string;
  /** Aktif rol kodu filtresidir. */
  role?: string;
  /** Aktiflik (null=tümü) filtresidir. */
  isActive?: boolean | null;
  /** Kilit durumu (null=tümü) filtresidir. */
  isLocked?: boolean | null;
  /** Mesai dışı kısıt (null=tümü) filtresidir. */
  clockLimit?: boolean | null;
  /** Başlangıç geçerlilik tarihi, UTC YYYY-MM-DD biçimindedir. */
  validFromDate?: string;
  /** Bitiş geçerlilik tarihi, UTC YYYY-MM-DD biçimindedir. */
  validToDate?: string;
  /** Oluşturulma tarihi, UTC YYYY-MM-DD biçimindedir. */
  createdDate?: string;
}

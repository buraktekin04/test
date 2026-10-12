/**
 * Kullanıcı görev yeri seçiminde kullanılan, aktif organizasyon birimi
 * kataloğunun HTTP yanıt modelidir. Rol/permission içermez.
 */
export interface OrganizationUnitOption {
  /** Organizasyon biriminin benzersiz GUID anahtarıdır. */
  id: string;
  /** Kurumsal organizasyon kodudur. */
  code: string;
  /** Birimin ekranda gösterilen adıdır. */
  name: string;
  /** Üst organizasyonun GUID'i, kök birimlerde null olabilir. */
  parentOrganizationUnitId: string | null;
}

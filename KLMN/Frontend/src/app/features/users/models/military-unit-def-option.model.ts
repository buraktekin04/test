/**
 * Kullanıcı görev yeri seçiminde kullanılan, aktif birlik
 * kataloğunun HTTP yanıt modelidir. Rol/permission içermez.
 */
export interface MilitaryUnitDefOption {
  /** Birliğin benzersiz GUID anahtarıdır. */
  id: string;
  /** Kurumsal birlik kodudur. */
  code: string;
  /** Birimin ekranda gösterilen adıdır. */
  name: string;
  /** Üst birlikun GUID'i, kök birimlerde null olabilir. */
  connectedTopUnity: string | null;
}

/**
 * Sidebar navigasyon elemanını temsil eder.
 */
export interface NavigationItem {
  /** Menüde kullanıcıya gösterilen başlık metnidir. */
  label: string;
  /** PrimeIcons kütüphanesinden gösterilecek simgenin CSS adıdır. */
  icon?: string;
  /** Tıklandığında açılacak Angular uygulama rotasıdır. */
  route?: string;
  /** Menü öğesini göstermek için aranan permission kodlarıdır. */
  requiredPermissions?: readonly string[];
  /** Birden fazla permission için tamamı veya herhangi biri koşulunu seçer. */
  permissionMode?: 'all' | 'any';
  /** Alt menüde gruplanmış iç içe navigasyon öğeleridir. */
  children?: readonly NavigationItem[];
}

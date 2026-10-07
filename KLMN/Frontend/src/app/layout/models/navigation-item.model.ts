/**
 * Sidebar navigasyon elemanını temsil eder.
 */
export interface NavigationItem {
  label: string;
  icon?: string;
  route?: string;
  requiredPermissions?: readonly string[];
  permissionMode?: 'all' | 'any';
  children?: readonly NavigationItem[];
}

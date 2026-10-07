/**
 * Oturum açmış kullanıcıya ait profil ve UI yetki bilgilerini temsil eder.
 */
export interface AuthUser {
  id: string;
  userName: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  organizationUnitId: string | null;
  roles: string[];
  permissions: string[];
}

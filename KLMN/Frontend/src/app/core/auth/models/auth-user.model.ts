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
  phoneNumber?: string | null;
  organizationUnitId: string | null;
  organizationUnitName?: string | null;
  lastLoginDate?: string | null;
  passwordChangedDate?: string | null;
  roles: string[];
  permissions: string[];
}

/**
 * Kullanıcı listeleme özet modelidir.
 */
export interface UserListItem {
  id: string;
  userName: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phoneNumber: string | null;
  organizationUnitId: string | null;
  organizationUnitCode: string | null;
  organizationUnitName: string | null;
  roles: string[];
  isActive: boolean;
  isLocked: boolean;
  createdDate: string;
  version: number;
}

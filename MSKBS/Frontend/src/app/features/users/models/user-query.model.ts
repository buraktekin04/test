/**
 * Kullanıcı sorgu modelidir.
 */
export interface UserQuery {
  search?: string;
  includeInactive: boolean;
  pageNumber: number;
  pageSize: number;
}

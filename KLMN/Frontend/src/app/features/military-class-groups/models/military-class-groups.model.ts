/** Askerî sınıf grubu DTO; BaseEntity audit ve xmin sürümü kullanır. */
export interface MilitaryClassGroupItem {
  id: string;
  code: string; name: string; shortName: string | null;
  forcesCommandId: string;
  forcesCommandName: string;
  validFrom: string | null; validTo: string | null;
  isActive: boolean; version: number; createdDate: string;
}
/** Create/Update ortak alanları. */
export interface MilitaryClassGroupFields {
  code: string; name: string; shortName: string | null;
  forcesCommandId: string;
  validFrom: string | null; validTo: string | null;
}
/** Backend tarafında arama, aktiflik ve sayfalama filtresi. */
export interface MilitaryClassGroupQuery {
  search: string; includeInactive: boolean; isActive: boolean | null;
  forcesCommandId: string | null;
  pageNumber: number; pageSize: number;
}
export interface UpdateMilitaryClassGroupRequest extends MilitaryClassGroupFields { id: string; version: number; }
export interface ChangeMilitaryClassGroupStatusRequest { id: string; version: number; isActive: boolean; }
export interface MilitaryClassGroupMutationResponse { id: string; version: number; }
export interface MilitaryClassGroupOption { id: string; code: string; name: string; }

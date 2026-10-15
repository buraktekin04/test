/** Kuvvet-sınıf-grup eşleştirme DTO; BaseEntity audit ve xmin sürümü kullanır. */
export interface MilitaryClassGroupRelationItem {
  id: string;
  forcesCommandId: string; forcesCommandName: string;
  militaryClassId: string; militaryClassName: string;
  militaryClassGroupId: string; militaryClassGroupName: string;
  isActive: boolean; version: number; createdDate: string;
}
/** Create/Update ortak alanları. */
export interface MilitaryClassGroupRelationFields {
  forcesCommandId: string;
  militaryClassId: string; militaryClassGroupId: string;
}
/** Backend tarafında arama, aktiflik ve sayfalama filtresi. */
export interface MilitaryClassGroupRelationQuery {
  search: string; includeInactive: boolean; isActive: boolean | null;
  forcesCommandId: string | null;
  militaryClassId: string | null; militaryClassGroupId: string | null;
  pageNumber: number; pageSize: number;
}
export interface UpdateMilitaryClassGroupRelationRequest extends MilitaryClassGroupRelationFields { id: string; version: number; }
export interface ChangeMilitaryClassGroupRelationStatusRequest { id: string; version: number; isActive: boolean; }
export interface MilitaryClassGroupRelationMutationResponse { id: string; version: number; }
export interface MilitaryClassGroupRelationOption { id: string; code: string; name: string; }
export interface MilitaryClassGroupRelationOptions {
  forcesCommands: MilitaryClassGroupRelationOption[];
  militaryClasses: MilitaryClassGroupRelationOption[];
  groups: MilitaryClassGroupRelationOption[];
}

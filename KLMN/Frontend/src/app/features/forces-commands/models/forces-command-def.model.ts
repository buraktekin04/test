export interface ForcesCommandDefItem {
  id: string; code: string; name: string;
  validFrom: string | null; validTo: string | null;
  isActive: boolean; createdDate: string; version: number; militaryUnitCount: number;
}
export interface ForcesCommandDefFields {
  name: string; code: string; validFrom: string | null; validTo: string | null;
}
export interface ForcesCommandDefQuery {
  sortField: string;
  sortOrder: number;
  search: string; includeInactive: boolean; isActive: boolean | null;
  pageNumber: number; pageSize: number;
}
export interface UpdateForcesCommandDefRequest extends ForcesCommandDefFields { id: string; version: number; }
export interface ChangeForcesCommandDefStatusRequest { id: string; version: number; isActive: boolean; }
export interface ForcesCommandDefMutationResponse { id: string; version: number; }

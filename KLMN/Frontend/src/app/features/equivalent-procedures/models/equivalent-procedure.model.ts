/** İki tetkik arasındaki eşdeğerliği BaseEntity sürümüyle temsil eder. */
export interface EquivalentProcedureItem {
 id:string;procedureDefId:string;procedureCode:string;procedureName:string;
 equivalentProcedureId:string;equivalentProcedureCode:string;equivalentProcedureName:string;
 isActive:boolean;version:number;createdDate:string;
}
export interface EquivalentProcedureOption {id:string;code:string;name:string;isActive:boolean;label:string;}
export interface EquivalentProcedureQuery {
 search:string;procedureId:string|null;includeInactive:boolean;isActive:boolean|null;
 pageNumber:number;pageSize:number;
}
export interface EquivalentProcedurePagedResult {
 items:EquivalentProcedureItem[];totalCount:number;pageNumber:number;pageSize:number;
}
export interface EquivalentProcedureMutationResponse{id:string;version:number;}
export interface EquivalentProcedureFields{procedureDefId:string;equivalentProcedureId:string;}

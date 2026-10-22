/** SYY Gereken Tetkikler ekranının bağımsız DTO'su. */
export interface SyyDiagnosisProcedureItem {
 id:string;version:number;isActive:boolean;createdDate:string;
 syyDiagnosisDefId:string;diagnosisName:string;
 procedureDefId:string;procedureName:string;
 startingAge:number;endingAge:number;gender:number;
}
export interface SyySelection {id:string;name:string;isActive:boolean;}
export interface SyyDiagnosisProcedureItemOptions {
 diagnoses:SyySelection[]; references:SyySelection[];
}
export interface SyyDiagnosisProcedureItemQuery {search:string;syyDiagnosisDefId:string|null;includeInactive:boolean;isActive:boolean|null;pageNumber:number;pageSize:number;}
export interface PagedItems<T>{items:T[];totalCount:number;pageNumber:number;pageSize:number;}

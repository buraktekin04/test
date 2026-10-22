/** SYY Gereken Branşlar ekranının bağımsız DTO'su. */
export interface SyyDiagnosisBranchItem {
 id:string;version:number;isActive:boolean;createdDate:string;
 syyDiagnosisDefId:string;diagnosisName:string;
 branchDefId:string;branchName:string;
 startingAge:number;endingAge:number;gender:number;
}
export interface SyySelection {id:string;name:string;isActive:boolean;}
export interface SyyDiagnosisBranchItemOptions {
 diagnoses:SyySelection[]; references:SyySelection[];
}
export interface SyyDiagnosisBranchItemQuery {search:string;syyDiagnosisDefId:string|null;includeInactive:boolean;isActive:boolean|null;pageNumber:number;pageSize:number;}
export interface PagedItems<T>{items:T[];totalCount:number;pageNumber:number;pageSize:number;}

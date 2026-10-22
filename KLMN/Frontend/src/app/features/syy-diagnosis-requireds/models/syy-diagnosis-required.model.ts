export interface SyyRequiredDetail{id:string;description:string;sortOrder:number;}
export interface SyyRequiredItem{
 id:string;syyDiagnosisDefId:string;diagnosisName:string;
 mustAll:boolean|null;mustOneOf:boolean|null;isActive:boolean;version:number;
 details:SyyRequiredDetail[];
}
export interface SyyRequiredOption{id:string;name:string;isActive:boolean;}
export interface SyyRequiredQuery{search:string;syyDiagnosisDefId:string|null;isActive:boolean|null;
 includeInactive:boolean;pageNumber:number;pageSize:number;}
export interface PageResult<T>{items:T[];totalCount:number;pageNumber:number;pageSize:number;}

/** SYY Tanı Sınıflandırma Çizelgesi ekranının bağımsız DTO'su. */
export interface SyyClassificationItem {
 id:string;version:number;isActive:boolean;createdDate:string;
 syyDiagnosisDefId:string;diagnosisName:string;
 forcesCommandDef:string;forcesCommandName:string;
 rankGroup:string;rankGroupDefinitionName:string;
 militaryClassGroup:string;militaryClassGroupDefinitionName:string;
 hcDecision:string;decisionName:string;
 validFrom:string|null;validTo:string|null;
}
export interface SyySelection {id:string;name:string;isActive:boolean;}
export interface SyyClassificationItemOptions {
 diagnoses:SyySelection[]; forces:SyySelection[]; ranks:SyySelection[]; classes:SyySelection[]; decisions:SyySelection[];
}
export interface SyyClassificationItemQuery {search:string;syyDiagnosisDefId:string|null;includeInactive:boolean;isActive:boolean|null;pageNumber:number;pageSize:number;}
export interface PagedItems<T>{items:T[];totalCount:number;pageNumber:number;pageSize:number;}

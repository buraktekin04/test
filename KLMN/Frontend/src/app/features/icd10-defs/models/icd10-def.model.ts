/** Icd10DefItem: ICD10Defs ekranına özel sunucu DTO'su. */
export interface Icd10DefItem {
 id:string;code:string;name:string;isActive:boolean;version:number;createdDate:string;
 
}
/** ICD10Defs filtreleri ve sunucu sayfalaması. */
export interface DefinitionQuery {
 search:string;includeInactive:boolean;isActive:boolean|null;pageNumber:number;pageSize:number;
}
export interface PageResult<T> {items:T[];totalCount:number;pageNumber:number;pageSize:number;}


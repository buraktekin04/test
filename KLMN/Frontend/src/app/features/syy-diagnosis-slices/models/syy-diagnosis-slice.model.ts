/** SyyDiagnosisSliceItem: SYYDiagnosisSlices ekranına özel sunucu DTO'su. */
export interface SyyDiagnosisSliceItem {
 id:string;code:number;name:string;isActive:boolean;version:number;createdDate:string;
 
}
/** SYYDiagnosisSlices filtreleri ve sunucu sayfalaması. */
export interface DefinitionQuery {
 search:string;includeInactive:boolean;isActive:boolean|null;pageNumber:number;pageSize:number;
}
export interface PageResult<T> {items:T[];totalCount:number;pageNumber:number;pageSize:number;}


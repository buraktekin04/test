/** SyyDiagnosisItem: SYYDiagnoses ekranına özel sunucu DTO'su. */
export interface SyyDiagnosisItem {
 id:string;code:string;name:string;isActive:boolean;version:number;createdDate:string;
  syyDiagnosisType:number;diagnosisTypeName:string;attachedRecNo:string|null;
 attachedDiagnosisName:string|null;eligible:boolean|null;slice:number|null;sliceName:string|null; validFrom:string|null;validTo:string|null;
}
/** SYYDiagnoses filtreleri ve sunucu sayfalaması. */
export interface DefinitionQuery {
 search:string;includeInactive:boolean;isActive:boolean|null;pageNumber:number;pageSize:number;
}
export interface PageResult<T> {items:T[];totalCount:number;pageNumber:number;pageSize:number;}
/** SYY Tanısı FK dropdown seçenekleri. */
export interface ItemOption<T> {value:T;label:string;isActive:boolean;}
export interface DiagnosisOptions {
 types:ItemOption<number>[];slices:ItemOption<number>[];parents:ItemOption<string>[];
}

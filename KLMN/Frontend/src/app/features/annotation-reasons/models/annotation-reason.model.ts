/** AnnotationReasonItem: AnnotationReasons ekranına özel sunucu DTO'su. */
export interface AnnotationReasonItem {
 id:string;code:string;name:string;isActive:boolean;version:number;createdDate:string;
  validFrom:string|null;validTo:string|null;
}
/** AnnotationReasons filtreleri ve sunucu sayfalaması. */
export interface DefinitionQuery {
 search:string;includeInactive:boolean;isActive:boolean|null;pageNumber:number;pageSize:number;
}
export interface PageResult<T> {items:T[];totalCount:number;pageNumber:number;pageSize:number;}


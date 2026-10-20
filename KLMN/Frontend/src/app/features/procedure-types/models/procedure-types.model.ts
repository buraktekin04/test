/** Tetkik Türü Tanımları ekranının veritabanı DTO'su. */
export interface ProcedureTypeItem {
 id:string;code:number;name:string;isActive:boolean;version:number;createdDate:string;
}
export interface ProcedureTypeQuery {
 search:string;includeInactive:boolean;isActive:boolean|null;pageNumber:number;pageSize:number;
}
export interface ProcedureTypeMutation {id:string;version:number;}

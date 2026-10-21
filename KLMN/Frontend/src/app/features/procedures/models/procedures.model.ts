/** Eski ProcedureDef alan isimleri korunarak geliştirilen Tetkik DTO'su. */
export interface ProcedureDefItem {
 id:string;procedureCode:string;name:string;
 requiresNumericResult:boolean;requiresReferenceRange:boolean;requiresDeviationInfo:boolean;
 validFrom:string|null;validTo:string|null;
 procedureType:number;procedureTypeName:string;
 isActive:boolean;createdDate:string;createdBy:string|null;
 updatedDate:string|null;updatedBy:string|null;version:number;
}
/** Tetkik kaydında gönderilen iş alanları. */
export interface ProcedureFields {
 procedureCode:string;name:string;
 requiresNumericResult:boolean;requiresReferenceRange:boolean;requiresDeviationInfo:boolean;
 validFrom:string|null;validTo:string|null;procedureType:number;
}
/** Tetkik Türü Tanımları ekranının dinamik seçeneği. */
export interface ProcedureTypeOption {code:number;name:string;isActive:boolean;}
/** Sunucu sayfalama ve aktiflik filtresi. */
export interface ProcedureQuery {search:string;includeInactive:boolean;isActive:boolean|null;pageNumber:number;pageSize:number;}
export interface UpdateProcedureDefRequest extends ProcedureFields{id:string;version:number;}
export interface ChangeProcedureDefStatusRequest{id:string;version:number;isActive:boolean;}
export interface ProcedureDefMutationResponse{id:string;version:number;}

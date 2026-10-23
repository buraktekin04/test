/** ICD10 SYY Tanı İstatistik Tanımları: her satır üç FK ve bir sayısal miktar içerir. */
export interface Icd10DiagnosticItem {
 id:string;icd10DefId:string;icd10Code:string;icd10Name:string;
 syyDiagnosisDefId:string;syyDiagnosisName:string;
 hcAdmissionTypeDefId:string;hcAdmissionTypeName:string;
 amount:number;isActive:boolean;version:number;createdDate:string;
}
export interface Icd10DiagnosticOption {id:string;label:string;isActive:boolean;}
export interface Icd10DiagnosticOptions {
 icd10s:Icd10DiagnosticOption[];syyDiagnoses:Icd10DiagnosticOption[];
 admissionTypes:Icd10DiagnosticOption[];
}
export interface Icd10DiagnosticQuery {
 search:string;includeInactive:boolean;isActive:boolean|null;
 pageNumber:number;pageSize:number;icd10DefId:string|null;
 syyDiagnosisDefId:string|null;hcAdmissionTypeDefId:string|null;
}
export interface PagedItems<T>{items:T[];totalCount:number;pageNumber:number;pageSize:number;}

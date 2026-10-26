/** SYY tanısı/kurul kararı/başvuru grubu/askerî sınıf istatistiği. */
export interface SyyDiagnosisDecisionItem {
 id:string;
 syyDiagnosisDefId:string;syyDiagnosisCode:string;syyDiagnosisName:string;
 hcDecisionDefId:string;hcDecisionCode:number;hcDecisionName:string;
 hcAdmissionTypeDefId:string;hcAdmissionTypeName:string;
 militaryClassDefId:string|null;militaryClassName:string|null;
 amount:number;isActive:boolean;version:number;createdDate:string;
}
export interface SyyDiagnosisDecisionFields {
 syyDiagnosisDefId:string;hcDecisionDefId:string;hcAdmissionTypeDefId:string;
 militaryClassDefId:string|null;amount:number;
}
export interface SyyDiagnosisDecisionQuery {
 search:string;includeInactive:boolean;isActive:boolean|null;pageNumber:number;pageSize:number;
 syyDiagnosisDefId:string|null;hcDecisionDefId:string|null;
 hcAdmissionTypeDefId:string|null;militaryClassDefId:string|null;
}
export interface SyyDiagnosisDecisionOption {id:string;label:string;isActive:boolean;}
export interface SyyDiagnosisDecisionOptions {
 syyDiagnoses:SyyDiagnosisDecisionOption[];decisions:SyyDiagnosisDecisionOption[];
 admissionTypes:SyyDiagnosisDecisionOption[];militaryClasses:SyyDiagnosisDecisionOption[];
}
export interface SyyDiagnosisDecisionPagedResult {
 items:SyyDiagnosisDecisionItem[];totalCount:number;pageNumber:number;pageSize:number;
}
export interface SyyDiagnosisDecisionMutation {id:string;version:number;}

/** Eski MSKBS sevk alanlarının yeni KLMN API sözleşmesi. */
export interface DispatchPerson {
 tcKimlikNo:string|null; yabanciKimlikNo:string|null;
 name:string; surname:string; motherName:string|null; fatherName:string|null;
 nationalityId:string|null; townOfBirthId:string|null; birthDate:string;
 gender:number|null; phoneNumber:string|null; address:string|null;
 birthPlaceText:string|null; birthPlaceCode:string|null;
}
export interface DispatchDetails {
 nthAdmission:number|null; dispatchDocumentDate:string|null;
 dispatchDocumentNo:string|null; reasonForExaminationDefId:string|null;
 dispatchScope:number; dispatchScopeCityId:string|null; dispatchScopeHospitalId:string|null;
 hcAdmissionTypeDefId:string|null; forcesCommandDefId:string|null;
 rankDefId:string|null; militaryClassDefId:string|null; militaryUnitDefId:string|null;
 militarySchoolDefId:string|null; senderMilitaryUnitDefId:string|null;
 senderMilitaryOfficeDefId:string|null; senderChairOther:string|null;
 militaryOfficeDefId:string|null; flyingMissionDefId:string|null;
 planeTypeDefId:string|null; registryNumber:string|null; lastNominationDate:string|null;
 militaryUnitDescription:string|null; purchaseId:number|null; description:string|null;
}
export interface DispatchItem {
 id:string; dispatchNo:number; personName:string; identityMasked:string;
 dispatchDate:string; dispatchDocumentDate:string|null; dispatchDocumentNo:string|null;
 admissionTypeName:string; currentState:number; isActive:boolean; version:number;
}
export interface DispatchResponse {
 id:string; dispatchNo:number; dispatchDate:string; currentState:number;
 isActive:boolean; version:number; person:DispatchPerson; dispatch:DispatchDetails;
}
export interface DispatchSaveResponse{id:string;dispatchNo:number;version:number;}
export interface DispatchOption{id:string;label:string;}
/** Başvuru grubunun seçeceği zorunlu alanlar ve yasaklanan muayene sebepleri. */
export interface DispatchAdmissionRules {
 forcesCommandRequired:boolean; rankRequired:boolean; militaryClassRequired:boolean;
 militaryUnitRequired:boolean; senderChairRequired:boolean; militaryOfficeRequired:boolean;
 registryNumberRequired:boolean; militarySchoolRequired:boolean; lastNominationDateRequired:boolean;
 nthAdmissionRequired:boolean; dispatchDocumentDateRequired:boolean; dispatchDocumentNoRequired:boolean;
 unusedReasonIds:string[];
}
export interface DispatchFileItem{id:string;fileName:string;contentType:string;fileSize:number;createdDate:string;}
export interface DispatchPage{items:DispatchItem[];totalCount:number;pageNumber:number;pageSize:number;}
export type OptionType='countries'|'towns'|'cities'|'hospitals'|'admissionTypes'|'reasons'|
 'forces'|'ranks'|'classes'|'units'|'schools'|'offices'|'missions'|'planes';

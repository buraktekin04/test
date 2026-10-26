/** Bağlı Kararlar: ana karar ve seçilen referans bilgileri. */
export interface HcDecisionRelationItem {
 id:string;hcDecisionDefId:string;decisionName:string;referenceId:string;referenceName:string;isActive:boolean;version:number;createdDate:string;
}
export interface HcDecisionRelationFields {hcDecisionDefId:string;referenceId:string;}
export interface HcDecisionRelationQuery {search:string;includeInactive:boolean;isActive:boolean|null;hcDecisionDefId:string|null;pageNumber:number;pageSize:number;}
export interface HcDecisionOption {id:string;label:string;isActive:boolean;}
export interface HcDecisionRelationOptions {decisions:HcDecisionOption[];references:HcDecisionOption[];}
export interface HcDecisionMutationResponse{id:string;version:number;}

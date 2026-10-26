/** Kurul kararının ortak geçerlilik, xmin ve iş kuralı bayrakları. */
export interface HcDecisionItem {
 id:string;code:number;name:string;
 timeInformationRequired:boolean;additionalDescriptionRequired:boolean;negativeDecision:boolean;approvalRequired:boolean;pertemApprovalRequired:boolean;diagnosisRequired:boolean;
 validFrom:string|null;validTo:string|null;isActive:boolean;version:number;createdDate:string;
 unusedReasons:string[];unusedAdmissionTypes:string[];unusedDecisions:string[];connectedDecisions:string[];
}
export type HcDecisionFields=Pick<HcDecisionItem,'code'|'name'|'timeInformationRequired'|'additionalDescriptionRequired'|'negativeDecision'|'approvalRequired'|'pertemApprovalRequired'|'diagnosisRequired'|'validFrom'|'validTo'>;
export interface HcDecisionQuery {search:string;includeInactive:boolean;isActive:boolean|null;pageNumber:number;pageSize:number;}
export interface HcDecisionMutationResponse {id:string;version:number;}
export interface HcDecisionOption {id:string;label:string;isActive:boolean;}
export interface HcDecisionRelationOptions {decisions:HcDecisionOption[];references:HcDecisionOption[];}
export interface HcDecisionRelationItem {id:string;hcDecisionDefId:string;decisionName:string;referenceId:string;referenceName:string;isActive:boolean;version:number;createdDate:string;}
export interface HcDecisionRelationFields {hcDecisionDefId:string;referenceId:string;}
export interface HcDecisionRelationQuery extends HcDecisionQuery {hcDecisionDefId:string|null;}

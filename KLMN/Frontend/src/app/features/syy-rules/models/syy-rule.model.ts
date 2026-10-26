/** Yeni tablo değil, SYYDiagnosisDef.Eligible=true satırı ve mevcut üç FK ilişkisinin görünümü. */
export interface SyyRuleBranch {id:string;branchName:string;startingAge:number;endingAge:number;gender:number;isActive:boolean;}
export interface SyyRuleProcedure {id:string;procedureName:string;startingAge:number;endingAge:number;gender:number;isActive:boolean;}
export interface SyyRuleApprovalDetail {id:string;description:string;sortOrder:number;}
export interface SyyRuleApproval {id:string;mustAll:boolean|null;mustOneOf:boolean|null;isActive:boolean;details:SyyRuleApprovalDetail[];}
export interface SyyRule {
 id:string;code:string;name:string;diagnosisTypeName:string;sliceName:string|null;
 validFrom:string|null;validTo:string|null;isActive:boolean;
 branches:SyyRuleBranch[];procedures:SyyRuleProcedure[];approvals:SyyRuleApproval[];
}
export interface SyyRuleQuery {search:string;includeInactive:boolean;pageNumber:number;pageSize:number;syyDiagnosisDefId:string|null;}
export interface SyyRulePagedResponse {items:SyyRule[];totalCount:number;pageNumber:number;pageSize:number;}

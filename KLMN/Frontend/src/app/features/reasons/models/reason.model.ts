/** Muayene sebebi; aktif alt ilişkilerin adları ana tabloda görünür. */
export interface ReasonItem {
 id:string; code:string; description:string;
 dispatchRequired:boolean; classificationExamination:boolean; objectionable:boolean;
 validFrom:string|null; validTo:string|null; isActive:boolean;version:number;createdDate:string;
 reportTypes:string[]; unusedAdmissionTypes:string[]; procedures:string[];branches:string[];
}
export interface ReasonFields {
 code:string;description:string;dispatchRequired:boolean;classificationExamination:boolean;objectionable:boolean;
 validFrom:string|null;validTo:string|null;
}
export interface ReasonQuery {
 search:string;includeInactive:boolean;isActive:boolean|null;pageNumber:number;pageSize:number;
}
export interface MutationResponse { id:string;version:number; }
export interface ReasonOption {id:string;label:string;isActive:boolean;}
export interface RelationOptions {reasons:ReasonOption[];references:ReasonOption[];}
export interface RelationItem {
 id:string;reasonForExaminationDefId:string;reasonDescription:string;reasonCode:string;
 referenceId:string;referenceName:string;isActive:boolean;version:number;createdDate:string;
 startingAge?:number;endingAge?:number;gender?:number;
}
export interface RelationFields {
 reasonForExaminationDefId:string;referenceId:string;
 startingAge?:number;endingAge?:number;gender?:number;
}
export interface RelationQuery extends ReasonQuery {reasonForExaminationDefId:string|null;}
export type RelationKind='reportTypes'|'unusedAdmissionTypes'|'procedures'|'branches';

/** SYY tanımları ortak DTO alanları. Alanlar backend response ile eşleşir. */
export interface DefinitionItem {
 id:string;code:string|number;name:string;isActive:boolean;version:number;createdDate:string;
 validFrom?:string|null;validTo?:string|null;
 syyDiagnosisType?:number;diagnosisTypeName?:string;
 attachedRecNo?:string|null;attachedDiagnosisName?:string|null;eligible?:boolean|null;
 slice?:number|null;sliceName?:string|null;
}
/** SYY seçicileri: KLMN'de tanı türü ve dilim dinamik, bağlı kayıt self-reference. */
export interface ItemOption<T> {value:T;label:string;isActive:boolean;}
export interface DiagnosisOptions {
 types:ItemOption<number>[];slices:ItemOption<number>[];parents:ItemOption<string>[];
}
export type ModuleKey='SYYDiagnoses'|'SYYDiagnosisTypes'|'SYYDiagnosisSlices'
 |'AnnotationReasons'|'PlaneTypes'|'FlyingMissions';
export type DefinitionKind='diagnosis'|'lookup'|'simple';
export interface DefinitionConfig {module:ModuleKey;path:string;title:string;entity:string;kind:DefinitionKind;}
export interface PageResult<T> {items:T[];totalCount:number;pageNumber:number;pageSize:number;}
export interface DefinitionQuery {search:string;includeInactive:boolean;isActive:boolean|null;pageNumber:number;pageSize:number;}

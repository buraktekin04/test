import type {DispatchDetails,DispatchPerson} from '../../dispatches/models/dispatch.model';

/** Provizyon verileri durum tanım tablosuyla ilişkilendirilir. */
export interface ProvisionStatusOption {code:number;name:string;sortOrder:number;}
export interface ProvisionItem {
 id:string;provisionNo:number;dispatchId:string;dispatchNo:number;
 provisionDate:string;hospitalId:string;hospitalName:string;currentState:number;
 withoutDispatch:number|null;personName:string;identityMasked:string;
 purchaseId:number|null;objectionableReportId:string|null;
 description:string|null;version:number;
}
export interface ProvisionState {id:string;state:number;createdDate:string;createdBy:string|null;}
export interface ProvisionDetail {
 id:string;provisionNo:number;dispatchId:string;dispatchNo:number;
 provisionDate:string;hospitalId:string;currentState:number;
 withoutDispatch:number|null;objectionableReportId:string|null;
 description:string|null;version:number;person:DispatchPerson;dispatch:DispatchDetails;
 states:ProvisionState[];
}
export interface ProvisionMutation {id:string;provisionNo:number;dispatchId:string;version:number;}
export interface ProvisionPage {items:ProvisionItem[];totalCount:number;pageNumber:number;pageSize:number;totalPages:number;}
export interface ProvisionFilter {
 dateFrom?:string;dateTo?:string;currentState?:number|null;
 provisionNo?:number|null;hospitalId?:string|null;
 tcKimlikNo?:string;yabanciKimlikNo?:string;name?:string;surname?:string;
 purchaseId?:number|null;
}

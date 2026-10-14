export interface PurchaseNumberDefItem {
  id: string;
  code: string;
  name: string;
  supplyDescriptionName: string;
  validFrom: string | null;
  validTo: string | null;
  healthReportEndDate: string | null;
  isActive: boolean;
  createdDate: string;
  createdBy: string | null;
  updatedDate: string | null;
  updatedBy: string | null;
  version: number;
}
export interface PurchaseNumberDefFields {
  code: string; name: string; supplyDescriptionName: string;
  validFrom: string | null; validTo: string | null; healthReportEndDate: string | null;
}
export interface PurchaseNumberDefQuery {
  search: string; includeInactive: boolean; isActive: boolean | null;
  supplyDescription: string;
  pageNumber: number; pageSize: number;
}
export interface UpdatePurchaseNumberDefRequest extends PurchaseNumberDefFields { id: string; version: number; }
export interface ChangePurchaseNumberDefStatusRequest { id: string; version: number; isActive: boolean; }
export interface PurchaseNumberDefMutationResponse { id: string; version: number; }

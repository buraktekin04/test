/** Her toplu sevk satırının başarı/hata sonucu; kimlik numarası maskeli gelir. */
export interface BulkDispatchRowResult {
 rowNumber:number;
 identityMasked:string;
 name:string;
 surname:string;
 dispatchId:string|null;
 dispatchNo:number|null;
 success:boolean;
 resultCode:number;
 message:string;
}
export interface BulkDispatchResponse {
 totalCount:number;
 successCount:number;
 failureCount:number;
 rows:BulkDispatchRowResult[];
}

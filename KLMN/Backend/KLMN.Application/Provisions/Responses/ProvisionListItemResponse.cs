namespace KLMN.Application.Provisions.Responses;

/// <summary>Provizyon sorgusunda maskeli kişi bilgisi ve temel durum.</summary>
public sealed record ProvisionListItemResponse(
    Guid Id,long ProvisionNo,Guid DispatchId,long DispatchNo,DateTime ProvisionDate,
    Guid HospitalId,string HospitalName,int CurrentState,int? WithoutDispatch,
    string PersonName,string IdentityMasked,int? PurchaseId,
    Guid? ObjectionableReportId,string? Description,uint Version);

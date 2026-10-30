namespace KLMN.Application.Dispatches.Responses;

/// <summary>Hassas kişisel verileri tam açığa çıkarmayan sevk liste satırı.</summary>
public sealed record DispatchListItemResponse(Guid Id,long DispatchNo,string PersonName,string IdentityMasked,
 DateTime DispatchDate,DateOnly? DispatchDocumentDate,string? DispatchDocumentNo,string AdmissionTypeName,
 int CurrentState,bool IsActive,uint Version);

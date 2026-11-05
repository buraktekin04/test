namespace KLMN.Application.Provisions.Responses;

/// <summary>Provizyonun eski sistem durum kodu ve tarihçesi.</summary>
public sealed record ProvisionStateResponse(Guid Id,int State,DateTime CreatedDate,Guid? CreatedBy);

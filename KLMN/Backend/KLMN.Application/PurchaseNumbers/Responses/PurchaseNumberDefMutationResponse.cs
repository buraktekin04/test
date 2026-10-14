namespace KLMN.Application.PurchaseNumbers.Responses;

/// <summary>PostgreSQL xmin versiyonuyla birlikte dönen güncelleme sonucu.</summary>
public sealed record PurchaseNumberDefMutationResponse(Guid Id, uint Version);

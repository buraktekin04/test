namespace KLMN.Application.Organizations.Responses;

/// <summary>Yeni kaydın kimliğini ve PostgreSQL xmin versiyonunu döndürür.</summary>
public sealed record OrganizationUnitMutationResponse(Guid Id, uint Version);

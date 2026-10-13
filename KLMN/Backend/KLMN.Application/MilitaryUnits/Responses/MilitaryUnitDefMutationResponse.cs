namespace KLMN.Application.MilitaryUnits.Responses;

/// <summary>Yeni kaydın kimliğini ve PostgreSQL xmin versiyonunu döndürür.</summary>
public sealed record MilitaryUnitDefMutationResponse(Guid Id, uint Version);

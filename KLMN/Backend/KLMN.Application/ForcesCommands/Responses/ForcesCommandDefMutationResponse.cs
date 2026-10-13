namespace KLMN.Application.ForcesCommands.Responses;

/// <summary>Yeni kaydın kimliğini ve PostgreSQL xmin versiyonunu döndürür.</summary>
public sealed record ForcesCommandDefMutationResponse(Guid Id, uint Version);

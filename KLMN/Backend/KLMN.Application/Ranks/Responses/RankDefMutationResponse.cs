namespace KLMN.Application.Ranks.Responses;

/// <summary>Kaydedilen rütbeın kimliği ve PostgreSQL xmin değeri.</summary>
public sealed record RankDefMutationResponse(Guid Id, uint Version);

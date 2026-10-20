namespace KLMN.Application.RankGroups.Responses;

/// <summary>Kaydedilen rütbe grubuın kimliği ve PostgreSQL xmin değeri.</summary>
public sealed record RankGroupDefMutationResponse(Guid Id, uint Version);

namespace KLMN.Application.HcDecisions.Responses;
/// <summary>Kaydın anahtarı ve PostgreSQL xmin sürümü.</summary>
public sealed record HcDecisionDefMutationResponse(Guid Id,uint Version);

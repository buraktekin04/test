namespace KLMN.Application.SYYDiagnosisDecisions.Responses;
/// <summary>PostgreSQL xmin ile güncel kayıt sürümü.</summary>
public sealed record SYYDiagnosisDecisionMutationResponse(Guid Id,uint Version);

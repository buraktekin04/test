namespace KLMN.Application.HcUnusedDecisions.Responses;
/// <summary>Birlikte Verilmeyecek Kararlar kaydının anahtarı ve xmin sürümü.</summary>
public sealed record HcUnusedDecisionMutationResponse(Guid Id,uint Version);

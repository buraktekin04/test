namespace KLMN.Application.HcDecisionConnections.Responses;
/// <summary>Bağlı Kararlar kaydının anahtarı ve xmin sürümü.</summary>
public sealed record HcConnectedDecisionMutationResponse(Guid Id,uint Version);

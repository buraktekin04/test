namespace KLMN.Application.ConnectedDecisions.Responses;

/// <summary>Kaydedilen kararın kimliği ve PostgreSQL xmin değeri.</summary>
public sealed record HcConnectedDecisionDefMutationResponse(Guid Id, uint Version);

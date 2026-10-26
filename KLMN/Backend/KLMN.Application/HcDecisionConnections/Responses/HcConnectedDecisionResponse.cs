namespace KLMN.Application.HcDecisionConnections.Responses;
/// <summary>Bağlı Kararlar için ana kurul kararı ve referans adlarını taşıyan DTO.</summary>
public sealed record HcConnectedDecisionResponse(Guid Id,Guid HcDecisionDefId,string DecisionName,Guid ReferenceId,string ReferenceName,bool IsActive,uint Version,DateTime CreatedDate);

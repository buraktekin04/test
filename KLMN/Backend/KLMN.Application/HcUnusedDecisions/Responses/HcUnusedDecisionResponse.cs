namespace KLMN.Application.HcUnusedDecisions.Responses;
/// <summary>Birlikte Verilmeyecek Kararlar için ana kurul kararı ve referans adlarını taşıyan DTO.</summary>
public sealed record HcUnusedDecisionResponse(Guid Id,Guid HcDecisionDefId,string DecisionName,Guid ReferenceId,string ReferenceName,bool IsActive,uint Version,DateTime CreatedDate);

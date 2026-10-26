namespace KLMN.Application.HcUnusedReasons.Responses;
/// <summary>Kullanılmayacak Muayene Sebepleri için ana kurul kararı ve referans adlarını taşıyan DTO.</summary>
public sealed record HcUnusedReasonForExamResponse(Guid Id,Guid HcDecisionDefId,string DecisionName,Guid ReferenceId,string ReferenceName,bool IsActive,uint Version,DateTime CreatedDate);

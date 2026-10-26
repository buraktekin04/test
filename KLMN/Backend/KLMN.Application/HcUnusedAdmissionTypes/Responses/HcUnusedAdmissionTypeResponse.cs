namespace KLMN.Application.HcUnusedAdmissionTypes.Responses;
/// <summary>Kullanılmayacak Başvuru Grupları için ana kurul kararı ve referans adlarını taşıyan DTO.</summary>
public sealed record HcUnusedAdmissionTypeResponse(Guid Id,Guid HcDecisionDefId,string DecisionName,Guid ReferenceId,string ReferenceName,bool IsActive,uint Version,DateTime CreatedDate);

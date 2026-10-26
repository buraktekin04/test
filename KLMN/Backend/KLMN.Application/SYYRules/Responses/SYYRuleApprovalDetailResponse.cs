namespace KLMN.Application.SYYRules.Responses;
/// <summary>Onay kuralı açıklama maddesinin salt okunur görünümü.</summary>
public sealed record SYYRuleApprovalDetailResponse(Guid Id,string Description,int SortOrder);

namespace KLMN.Application.SYYRules.Responses;
/// <summary>Onay kuralı ve sıralı alt maddeleri.</summary>
public sealed record SYYRuleApprovalResponse(Guid Id,bool? MustAll,bool? MustOneOf,bool IsActive,
 IReadOnlyList<SYYRuleApprovalDetailResponse> Details);

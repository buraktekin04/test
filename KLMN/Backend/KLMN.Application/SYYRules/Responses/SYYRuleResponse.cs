namespace KLMN.Application.SYYRules.Responses;
/// <summary>Eligible=true olan mevcut SYY tanısı ve ona bağlı üç tanım türü.</summary>
public sealed record SYYRuleResponse(Guid Id,string Code,string Name,string DiagnosisTypeName,
 string? SliceName,DateTime? ValidFrom,DateTime? ValidTo,bool IsActive,
 IReadOnlyList<SYYRuleBranchResponse> Branches,
 IReadOnlyList<SYYRuleProcedureResponse> Procedures,
 IReadOnlyList<SYYRuleApprovalResponse> Approvals);

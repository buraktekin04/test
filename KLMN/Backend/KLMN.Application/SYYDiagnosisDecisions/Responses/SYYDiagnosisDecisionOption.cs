namespace KLMN.Application.SYYDiagnosisDecisions.Responses;
/// <summary>Yalnızca seçime uygun tanım ve aktiflik bilgisi.</summary>
public sealed record SYYDiagnosisDecisionOption(Guid Id,string Label,bool IsActive);

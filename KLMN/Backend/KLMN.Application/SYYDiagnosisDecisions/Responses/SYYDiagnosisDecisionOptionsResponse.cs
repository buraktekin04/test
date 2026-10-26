namespace KLMN.Application.SYYDiagnosisDecisions.Responses;
/// <summary>İki ekranın sunucudan aldığı dört FK seçeneği.</summary>
public sealed record SYYDiagnosisDecisionOptionsResponse(
 IReadOnlyList<SYYDiagnosisDecisionOption> SYYDiagnoses,
 IReadOnlyList<SYYDiagnosisDecisionOption> Decisions,
 IReadOnlyList<SYYDiagnosisDecisionOption> AdmissionTypes,
 IReadOnlyList<SYYDiagnosisDecisionOption> MilitaryClasses);

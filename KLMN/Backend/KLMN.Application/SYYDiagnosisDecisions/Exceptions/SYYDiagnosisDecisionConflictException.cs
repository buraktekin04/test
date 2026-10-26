namespace KLMN.Application.SYYDiagnosisDecisions.Exceptions;
/// <summary>Birden çok FK ya da benzersiz eşleşme için iş kuralı ihlali.</summary>
public sealed class SYYDiagnosisDecisionConflictException(string message):Exception(message);

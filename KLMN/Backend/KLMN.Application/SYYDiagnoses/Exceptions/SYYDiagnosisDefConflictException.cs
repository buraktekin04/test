namespace KLMN.Application.SYYDiagnoses.Exceptions;
/// <summary>SYY Tanı Listesi (M/D/F) iş kuralı ve referans çakışması.</summary>
public sealed class SYYDiagnosisDefConflictException(string message):Exception(message);

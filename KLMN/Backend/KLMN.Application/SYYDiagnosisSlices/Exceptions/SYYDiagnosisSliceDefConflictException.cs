namespace KLMN.Application.SYYDiagnosisSlices.Exceptions;
/// <summary>SYY Dilim Tanımları iş kuralı ve referans çakışması.</summary>
public sealed class SYYDiagnosisSliceDefConflictException(string message):Exception(message);

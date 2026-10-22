namespace KLMN.Application.SYYDiagnosisSlices.Exceptions;
/// <summary>SYY Dilim Tanımları kaydı bulunamadı.</summary>
public sealed class SYYDiagnosisSliceDefNotFoundException():Exception("Kayıt bulunamadı.");
/// <summary>SYY Dilim Tanımları iş kuralı ve referans çakışması.</summary>
public sealed class SYYDiagnosisSliceDefConflictException(string message):Exception(message);

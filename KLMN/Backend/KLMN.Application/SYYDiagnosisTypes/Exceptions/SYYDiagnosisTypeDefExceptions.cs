namespace KLMN.Application.SYYDiagnosisTypes.Exceptions;
/// <summary>SYY Tanı Türü Tanımları kaydı bulunamadı.</summary>
public sealed class SYYDiagnosisTypeDefNotFoundException():Exception("Kayıt bulunamadı.");
/// <summary>SYY Tanı Türü Tanımları iş kuralı ve referans çakışması.</summary>
public sealed class SYYDiagnosisTypeDefConflictException(string message):Exception(message);

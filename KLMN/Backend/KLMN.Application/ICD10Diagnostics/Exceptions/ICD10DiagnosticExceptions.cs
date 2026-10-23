namespace KLMN.Application.ICD10Diagnostics.Exceptions;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları: kayıt bulunamadı (404).</summary>
public sealed class ICD10DiagnosticNotFoundException():Exception("ICD10 SYY Tanı İstatistik Tanımları kaydı bulunamadı.");
/// <summary>ICD10 SYY Tanı İstatistik Tanımları: iş kuralı veya referans çakışması (409).</summary>
public sealed class ICD10DiagnosticConflictException(string message):Exception(message);

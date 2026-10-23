namespace KLMN.Application.ICD10Defs.Exceptions;
/// <summary>ICD10 Tanımları: iş kuralı veya referans çakışması (409).</summary>
public sealed class ICD10DefConflictException(string message):Exception(message);

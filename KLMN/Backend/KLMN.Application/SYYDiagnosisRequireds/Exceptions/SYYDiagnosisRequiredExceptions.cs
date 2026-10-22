namespace KLMN.Application.SYYDiagnosisRequireds.Exceptions;
public sealed class SYYDiagnosisRequiredNotFoundException():Exception("SYY onay kuralı bulunamadı.");
public sealed class SYYDiagnosisRequiredConflictException(string message):Exception(message);

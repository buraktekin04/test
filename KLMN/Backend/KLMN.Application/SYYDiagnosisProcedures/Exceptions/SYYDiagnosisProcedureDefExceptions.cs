namespace KLMN.Application.SYYDiagnosisProcedures.Exceptions;
public sealed class SYYDiagnosisProcedureDefNotFoundException():Exception("SYY Gereken Tetkikler kaydı bulunamadı.");
public sealed class SYYDiagnosisProcedureDefConflictException(string message):Exception(message);

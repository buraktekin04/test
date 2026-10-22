namespace KLMN.Application.SYYDiagnosisBranches.Exceptions;
public sealed class SYYDiagnosisBranchDefNotFoundException():Exception("SYY Gereken Branşlar kaydı bulunamadı.");
public sealed class SYYDiagnosisBranchDefConflictException(string message):Exception(message);

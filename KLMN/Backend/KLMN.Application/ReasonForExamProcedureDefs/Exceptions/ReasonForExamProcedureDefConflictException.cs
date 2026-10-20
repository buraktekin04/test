namespace KLMN.Application.ReasonForExamProcedureDefs.Exceptions;
/// <summary>Gereken Tetkikler ilişki çakışması: HTTP 409.</summary>
public sealed class ReasonForExamProcedureDefConflictException(string message):Exception(message);

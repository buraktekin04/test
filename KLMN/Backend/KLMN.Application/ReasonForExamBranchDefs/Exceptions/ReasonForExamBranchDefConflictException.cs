namespace KLMN.Application.ReasonForExamBranchDefs.Exceptions;
/// <summary>Muayene Gereken Branşlar ilişki çakışması: HTTP 409.</summary>
public sealed class ReasonForExamBranchDefConflictException(string message):Exception(message);

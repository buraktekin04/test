namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Exceptions;
/// <summary>Kullanılmayacak Hasta Grupları ilişki çakışması: HTTP 409.</summary>
public sealed class ReasonForExamUnusedHcAdmissionTypeConflictException(string message):Exception(message);

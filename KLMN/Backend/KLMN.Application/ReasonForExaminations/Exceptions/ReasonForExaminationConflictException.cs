namespace KLMN.Application.ReasonForExaminations.Exceptions;
/// <summary>Muayene sebebi iş kuralı ihlali; HTTP 409.</summary>
public sealed class ReasonForExaminationConflictException(string message):Exception(message);

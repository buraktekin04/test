namespace KLMN.Application.ReasonForExamReportTypes.Exceptions;
/// <summary>Muayene Sebebi Rapor Türleri ilişki çakışması: HTTP 409.</summary>
public sealed class ReasonForExamReportTypeConflictException(string message):Exception(message);

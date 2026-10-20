namespace KLMN.Application.ReportTypes.Exceptions;

/// <summary>Rapor Türünda tekil kod ihlali: HTTP 409.</summary>
public sealed class ReportTypeConflictException(string message) : Exception(message);

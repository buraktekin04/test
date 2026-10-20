namespace KLMN.Application.HospitalReportTypes.Exceptions;

/// <summary>Sağlık Tesisi Rapor Türünda tekil kod ihlali: HTTP 409.</summary>
public sealed class HospitalReportTypeConflictException(string message) : Exception(message);

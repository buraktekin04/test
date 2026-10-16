namespace KLMN.Application.AdmissionTypes.Exceptions;

/// <summary>Tekil sayısal kod ihlali için HTTP 409 hatası.</summary>
public sealed class AdmissionTypeConflictException(string message) : Exception(message);

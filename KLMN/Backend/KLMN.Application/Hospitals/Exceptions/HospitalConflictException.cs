namespace KLMN.Application.Hospitals.Exceptions;

/// <summary>Sağlık Tesisinda tekil kod ihlali: HTTP 409.</summary>
public sealed class HospitalConflictException(string message) : Exception(message);

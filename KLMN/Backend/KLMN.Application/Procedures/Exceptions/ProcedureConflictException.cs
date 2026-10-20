namespace KLMN.Application.Procedures.Exceptions;

/// <summary>Tetkiknda tekil kod ihlali: HTTP 409.</summary>
public sealed class ProcedureConflictException(string message) : Exception(message);

namespace KLMN.Application.ConnectedDecisions.Exceptions;

/// <summary>Bağlı kurul kararında tekil kod ihlali: HTTP 409.</summary>
public sealed class ConnectedDecisionConflictException(string message) : Exception(message);

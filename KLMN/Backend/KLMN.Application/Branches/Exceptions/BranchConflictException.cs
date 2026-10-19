namespace KLMN.Application.Branches.Exceptions;

/// <summary>Branşnda tekil kod ihlali: HTTP 409.</summary>
public sealed class BranchConflictException(string message) : Exception(message);

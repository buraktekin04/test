namespace KLMN.Application.Ranks.Exceptions;

/// <summary>Rütbenda tekil kod ihlali: HTTP 409.</summary>
public sealed class RankConflictException(string message) : Exception(message);

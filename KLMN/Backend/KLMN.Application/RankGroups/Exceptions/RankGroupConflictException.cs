namespace KLMN.Application.RankGroups.Exceptions;

/// <summary>Rütbe Grubunda tekil kod ihlali: HTTP 409.</summary>
public sealed class RankGroupConflictException(string message) : Exception(message);

namespace KLMN.Application.Provisions.Exceptions;

/// <summary>Provizyon iş kuralı ve kimlik çakışması.</summary>
public sealed class ProvisionConflictException(string message) : Exception(message);

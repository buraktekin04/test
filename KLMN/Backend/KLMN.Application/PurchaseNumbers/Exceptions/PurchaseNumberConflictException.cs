namespace KLMN.Application.PurchaseNumbers.Exceptions;

/// <summary>Benzersizlik veya iş kuralı ihlali.</summary>
public sealed class PurchaseNumberConflictException(string message) : Exception(message);

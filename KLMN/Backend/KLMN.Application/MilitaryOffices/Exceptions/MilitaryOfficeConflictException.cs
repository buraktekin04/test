namespace KLMN.Application.MilitaryOffices.Exceptions;

/// <summary>Tekil kod veya aktif şehir ilişkisi iş kuralının ihlalidir.</summary>
public sealed class MilitaryOfficeConflictException(string message) : Exception(message);

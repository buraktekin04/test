namespace KLMN.Application.MilitaryClassGroups.Exceptions;

/// <summary>Askerî sınıf grubu için benzersiz kod veya referans kuralı ihlalidir.</summary>
public sealed class MilitaryClassGroupConflictException(string message) : Exception(message);

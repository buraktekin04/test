namespace KLMN.Application.MilitaryClasses.Exceptions;

/// <summary>Askerî sınıf için benzersiz kod veya kural çakışması.</summary>
public sealed class MilitaryClassConflictException(string message) : Exception(message);

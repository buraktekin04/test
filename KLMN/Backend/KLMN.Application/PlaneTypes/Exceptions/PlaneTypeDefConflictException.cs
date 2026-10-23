namespace KLMN.Application.PlaneTypes.Exceptions;
/// <summary>Uçuşa Uçak Tipi Tanımları iş kuralı ve referans çakışması.</summary>
public sealed class PlaneTypeDefConflictException(string message):Exception(message);

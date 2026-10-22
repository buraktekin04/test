namespace KLMN.Application.PlaneTypes.Exceptions;
/// <summary>Uçuşa Uçak Tipi Tanımları kaydı bulunamadı.</summary>
public sealed class PlaneTypeDefNotFoundException():Exception("Kayıt bulunamadı.");
/// <summary>Uçuşa Uçak Tipi Tanımları iş kuralı ve referans çakışması.</summary>
public sealed class PlaneTypeDefConflictException(string message):Exception(message);

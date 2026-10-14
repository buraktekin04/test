namespace KLMN.Application.Towns.Exceptions;
/// <summary>İlçe tanımına ait sorgu, yanıt veya iş kuralı bileşeni.</summary>
public sealed class TownConflictException(string message) : Exception(message);

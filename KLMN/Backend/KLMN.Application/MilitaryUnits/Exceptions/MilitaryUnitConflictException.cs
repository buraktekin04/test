namespace KLMN.Application.MilitaryUnits.Exceptions;

/// <summary>Hiyerarşi, benzersizlik veya bağlı kayıt kuralı ihlalinde 409 hatasıdır.</summary>
public sealed class MilitaryUnitConflictException(string message) : Exception(message);

namespace KLMN.Application.MilitaryUnits.Exceptions;

/// <summary>Soft-delete edilmiş veya bulunamayan birlik kaydı için 404 hatasıdır.</summary>
public sealed class MilitaryUnitNotFoundException() : Exception("Birlik bulunamadı.");

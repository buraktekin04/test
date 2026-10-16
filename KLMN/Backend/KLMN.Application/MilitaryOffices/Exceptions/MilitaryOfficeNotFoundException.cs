namespace KLMN.Application.MilitaryOffices.Exceptions;

/// <summary>Silinmiş veya bulunamayan şube için not-found hatası.</summary>
public sealed class MilitaryOfficeNotFoundException()
    : Exception("Askerlik şubesi kaydı bulunamadı.");

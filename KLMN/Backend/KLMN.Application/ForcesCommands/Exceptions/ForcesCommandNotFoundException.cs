namespace KLMN.Application.ForcesCommands.Exceptions;

/// <summary>Soft-delete edilmiş veya bulunamayan kuvvet komutanlığı kaydı için 404 hatasıdır.</summary>
public sealed class ForcesCommandNotFoundException() : Exception("Kuvvet komutanlığı bulunamadı.");

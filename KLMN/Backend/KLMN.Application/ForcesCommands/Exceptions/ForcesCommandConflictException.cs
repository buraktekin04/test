namespace KLMN.Application.ForcesCommands.Exceptions;

/// <summary>Hiyerarşi, benzersizlik veya bağlı kayıt kuralı ihlalinde 409 hatasıdır.</summary>
public sealed class ForcesCommandConflictException(string message) : Exception(message);

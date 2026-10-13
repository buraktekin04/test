namespace KLMN.Application.Organizations.Exceptions;

/// <summary>Hiyerarşi, benzersizlik veya bağlı kayıt kuralı ihlalinde 409 hatasıdır.</summary>
public sealed class OrganizationConflictException(string message) : Exception(message);

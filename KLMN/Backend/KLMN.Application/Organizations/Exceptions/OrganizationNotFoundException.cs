namespace KLMN.Application.Organizations.Exceptions;

/// <summary>Soft-delete edilmiş veya bulunamayan organizasyon kaydı için 404 hatasıdır.</summary>
public sealed class OrganizationNotFoundException() : Exception("Organizasyon birimi bulunamadı.");

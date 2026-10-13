namespace KLMN.Application.Organizations.Responses;

/// <summary>Üst birim dropdown'ına sunulan aktif, silinmemiş birim seçeneğidir.</summary>
public sealed record OrganizationParentOptionResponse(Guid Id, string Code, string Name, Guid? ParentOrganizationUnitId);

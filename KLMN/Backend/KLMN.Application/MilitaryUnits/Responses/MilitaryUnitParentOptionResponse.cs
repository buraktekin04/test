namespace KLMN.Application.MilitaryUnits.Responses;

/// <summary>Üst birim dropdown'ına sunulan aktif, silinmemiş birim seçeneğidir.</summary>
public sealed record MilitaryUnitParentOptionResponse(Guid Id, string Code, string Name, Guid? ConnectedTopUnity);

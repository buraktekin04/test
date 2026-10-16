namespace KLMN.Application.MilitaryUnits.Responses;

/// <summary>Üst birlik seçeneğidir; filtre görünümünde kuvvet adı da gösterilebilir.</summary>
public sealed record MilitaryUnitParentOptionResponse(
    Guid Id, string Code, string Name, Guid? ConnectedTopUnity, string? ForceName = null);

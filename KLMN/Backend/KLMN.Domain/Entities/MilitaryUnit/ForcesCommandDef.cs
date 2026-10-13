using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.MilitaryUnit;

/// <summary>Birliklerin bağlı olduğu kuvvet komutanlığı tanımıdır.</summary>
public sealed class ForcesCommandDef : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    /// <summary>UTC başlangıç dahildir; null ise başlangıç sınırı yoktur.</summary>
    public DateTime? ValidFrom { get; set; }
    /// <summary>UTC bitiş hariçtir; null ise süresizdir.</summary>
    public DateTime? ValidTo { get; set; }
    public ICollection<MilitaryUnitDef> MilitaryUnits { get; set; } = new List<MilitaryUnitDef>();
}

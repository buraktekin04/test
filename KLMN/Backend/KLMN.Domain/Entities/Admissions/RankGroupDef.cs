using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Admissions;

/// <summary>Rütbe Grubu tanımı. Audit, aktiflik ve geçerlilik alanları ortak entity'dedir.</summary>
public sealed class RankGroupDef : EffectiveDatedEntity
{
    /// <summary>Silinmemiş kayıtlar arasında benzersiz rütbe grubu kodu.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Rütbe grubunun adı.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Rütbe grubunun kısa adı.</summary>
    public string? ShortName { get; set; }

    /// <summary>Bu gruba bağlı rütbeler.</summary>
    public ICollection<RankDef> Ranks { get; set; } = new List<RankDef>();
}

using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.MilitaryUnit;

/// <summary>
/// Belirli kuvvet komutanlığına ait askerî sınıf grubu tanımı.
/// Eski ObjectId/audit/IsActive/xmin tekrar yazılmaz; nullable tarihler ortak alandır.
/// </summary>
public sealed class MilitaryClassGroupDef : EffectiveDatedEntity
{
    /// <summary>Grubun kuvvet içinde benzersiz teknik kodu.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Grup adı.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Grubun kısa adı.</summary>
    public string? ShortName { get; set; }

    /// <summary>Grubun mutlaka bağlı olduğu kuvvet komutanlığı.</summary>
    public Guid ForcesCommandId { get; set; }

    /// <summary>Silme davranışı Restrict olan kuvvet navigasyonu.</summary>
    public ForcesCommandDef ForcesCommand { get; set; } = null!;

    /// <summary>Bu gruba bağlanmış sınıfların kuvvet bazlı eşleştirmeleri.</summary>
    public ICollection<MilitaryClassGroupRelation> Relations { get; set; } = new List<MilitaryClassGroupRelation>();
}

using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.MilitaryUnit;

/// <summary>
/// Bir askerî sınıfın belirli kuvvette hangi sınıf grubuna bağlı olduğunu
/// gösteren üçlü ilişki tablosu; audit/soft-delete/xmin BaseEntity'den gelir.
/// </summary>
public sealed class MilitaryClassGroupRelation : BaseEntity
{
    /// <summary>İlişkinin kuvvet komutanlığı.</summary>
    public Guid ForcesCommandId { get; set; }

    /// <summary>İlişkinin askerî sınıfı.</summary>
    public Guid MilitaryClassId { get; set; }

    /// <summary>İlişkinin seçilen askerî sınıf grubu.</summary>
    public Guid MilitaryClassGroupId { get; set; }

    /// <summary>Kuvvet komutanlığı navigasyonu.</summary>
    public ForcesCommandDef ForcesCommand { get; set; } = null!;

    /// <summary>Askerî sınıf navigasyonu.</summary>
    public MilitaryClassDef MilitaryClass { get; set; } = null!;

    /// <summary>Grup navigasyonu.</summary>
    public MilitaryClassGroupDef MilitaryClassGroup { get; set; } = null!;
}

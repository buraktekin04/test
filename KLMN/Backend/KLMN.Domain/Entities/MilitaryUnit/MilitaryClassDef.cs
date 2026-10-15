using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.MilitaryUnit;

/// <summary>
/// Askerî sınıf tanımıdır. Eski Type, MpvtCode ve Town alanları yeni
/// sistemde kullanılmadığından bu entity'de yer almaz.
/// Aktif/pasif durumu yalnızca BaseEntity.IsActive üzerinden yönetilir.
/// Kimlik, audit, soft-delete ve xmin BaseEntity'den;
/// geçerlilik tarihleri EffectiveDatedEntity'den gelir.
/// </summary>
public sealed class MilitaryClassDef : EffectiveDatedEntity
{
    /// <summary>Silinmemiş kayıtlar arasında benzersiz teknik sınıf kodu.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Askerî sınıfın tam adı.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>İsteğe bağlı kısa ad.</summary>
    public string? ShortName { get; set; }

    /// <summary>Harici sistemlerde kullanılan isteğe bağlı kod.</summary>
    public string? ExternalCode { get; set; }

    /// <summary>Bu sınıfın kuvvet bazlı grup eşleştirmeleri.</summary>
    public ICollection<MilitaryClassGroupRelation> MilitaryClassGroupRelations { get; set; } = new List<MilitaryClassGroupRelation>();

}

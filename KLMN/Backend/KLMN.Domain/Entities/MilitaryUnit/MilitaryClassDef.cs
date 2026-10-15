using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.MilitaryUnit;

/// <summary>
/// Askerî sınıf tanımıdır. Eski Type, MpvtCode ve Town alanları yeni
/// sistemde kullanılmadığından bu entity'de yer almaz.
/// Kimlik, audit, aktiflik, soft-delete ve xmin BaseEntity'den;
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

    /// <summary>
    /// Eski modelin nullable Active alanını veri kaybını önlemek için taşır.
    /// BaseEntity.IsActive kayıt pasifleştirmesinden BAĞIMSIZDIR.
    /// </summary>
    public bool? Active { get; set; }
}

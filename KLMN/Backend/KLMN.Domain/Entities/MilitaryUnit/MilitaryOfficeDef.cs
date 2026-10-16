using KLMN.Domain.Common;
using KLMN.Domain.Entities.Geography;

namespace KLMN.Domain.Entities.MilitaryUnit;

/// <summary>
/// Askerlik şubesi tanımıdır. Audit, aktiflik, silme ve xmin BaseEntity'den;
/// UTC geçerlilik tarihleri EffectiveDatedEntity'den kalıtımla gelir.
/// RelatedCity kullanılmaz; yalnız CityDef ilişkisi tutulur.
/// </summary>
public sealed class MilitaryOfficeDef : EffectiveDatedEntity
{
    /// <summary>Silinmemiş tanımlar arasında tekil şube kodu.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Askerlik şubesinin adı.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Eski sistemle eşleştirme amacıyla kullanılan isteğe bağlı kod.</summary>
    public string? ExternalCode { get; set; }

    /// <summary>Eski City nullable GUID alanının CityDef FK karşılığı.</summary>
    public Guid? CityId { get; set; }

    /// <summary>Şubenin bağlı olduğu şehir; seçilmesi zorunlu değildir.</summary>
    public CityDef? City { get; set; }
}

using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Geography;

/// <summary>
/// İlçe tanımını tutar. Kimlik, audit, aktiflik, soft-delete ve xmin
/// alanları tekrar yazılmadan ortak BaseEntity'den miras alınır.
/// </summary>
public sealed class TownDef : BaseEntity
{
    /// <summary>Silinmemiş ilçeler arasında benzersiz teknik kod.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Kullanıcıya gösterilen ilçe adı.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Dış sistem eşleştirme kodu; isteğe bağlıdır.</summary>
    public string? ExternalCode { get; set; }

    /// <summary>İlçenin bağlı olduğu şehir (eski TownDef.CityDef); isteğe bağlıdır.</summary>
    public Guid? CityId { get; set; }

    /// <summary>Şehir FK navigasyonu; silme davranışı Restrict'tir.</summary>
    public CityDef? City { get; set; }

    /// <summary>İsteğe bağlı nüfus/MERNİS kodu.</summary>
    public int? MernisCode { get; set; }
}

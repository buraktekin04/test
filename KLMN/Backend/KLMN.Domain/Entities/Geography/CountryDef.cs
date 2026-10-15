using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Geography;

/// <summary>Eski CountryDef kayıtları; ortak audit, aktiflik, soft-delete ve xmin BaseEntity'dedir.</summary>
public sealed class CountryDef : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    /// <summary>Ülkenin kısa harf kodu (örn. TR, DE, USA); harici entegrasyon kodundan bağımsızdır.</summary>
    public string? ShortCode { get; set; }

    /// <summary>Harici sistem entegrasyonlarının kodudur; ShortCode yerine kullanılmaz.</summary>
    public string? ExternalCode { get; set; }
    /// <summary>İlçe/yerleşim yeri seçiminin gerekli olup olmadığını tanımlar; null bilinmiyor.</summary>
    public bool? IsTownRequired { get; set; }
    public ICollection<CityDef> Cities { get; set; } = new List<CityDef>();
}

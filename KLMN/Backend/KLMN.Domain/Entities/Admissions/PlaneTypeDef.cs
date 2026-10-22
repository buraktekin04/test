using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>Uçuşa Uçak Tipi Tanımları ekranının tanım kaydı; ortak audit ve geçerlilik alanları kalıtımdadır.</summary>
public sealed class PlaneTypeDef : EffectiveDatedEntity
{
    /// <summary>Kod.</summary>
    public string Code { get; set; } = string.Empty;
    /// <summary>Ad.</summary>
    public string Name { get; set; } = string.Empty;
}

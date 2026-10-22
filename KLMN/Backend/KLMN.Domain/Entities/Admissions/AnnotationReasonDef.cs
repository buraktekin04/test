using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>Sevk Sebebi Tanımları ekranının tanım kaydı; ortak audit ve geçerlilik alanları kalıtımdadır.</summary>
public sealed class AnnotationReasonDef : EffectiveDatedEntity
{
    /// <summary>Kod.</summary>
    public string Code { get; set; } = string.Empty;
    /// <summary>Ad.</summary>
    public string Name { get; set; } = string.Empty;
}

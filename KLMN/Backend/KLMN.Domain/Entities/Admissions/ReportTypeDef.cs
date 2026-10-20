using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Admissions;

/// <summary>Rapor Türü tanımı. Audit, aktiflik ve geçerlilik alanları ortak entity'dedir.</summary>
public sealed class ReportTypeDef : EffectiveDatedEntity
{
    /// <summary>Silinmemiş kayıtlar arasında benzersiz rapor türü kodu.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Rapor türünün adı.</summary>
    public string Name { get; set; } = string.Empty;
}

using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.PurchaseNumbers;

/// <summary>Alım numarası tanımı. Ortak audit/aktiflik/xmin BaseEntity'den gelir.</summary>
public sealed class PurchaseNumberDef : EffectiveDatedEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    // ValidFrom/ValidTo, ortak EffectiveDatedEntity sınıfından gelir.
    /// <summary>Sağlık raporu geçerlilik bitişi (UTC, opsiyonel).</summary>
    public DateTime? HealthReportEndDate { get; set; }
    public string SupplyDescriptionName { get; set; } = string.Empty;
}

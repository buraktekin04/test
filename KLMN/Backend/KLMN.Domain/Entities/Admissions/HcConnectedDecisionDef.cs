using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Admissions;

/// <summary>
/// Bağlı kurul karar tanımı. Sadece Code ve Name modele özgüdür;
/// kimlik/audit/aktiflik/soft-delete/xmin BaseEntity, tarih aralığı
/// ise EffectiveDatedEntity üzerinden gelir.
/// </summary>
public sealed class HcConnectedDecisionDef : EffectiveDatedEntity
{
    /// <summary>Silinmemiş kayıtlar arasında benzersiz karar kodu.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Bağlı kurul kararının adı.</summary>
    public string Name { get; set; } = string.Empty;
}

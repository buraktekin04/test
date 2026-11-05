using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Admissions;

/// <summary>Eski ProvisionState: her durum değişikliği ayrı auditli satırdır.</summary>
public sealed class ProvisionState : BaseEntity
{
    public Guid ProvisionId { get; set; }
    public Provision Provision { get; set; } = null!;
    /// <summary>Provizyon durum kodu: 1=Yeni, 2=Rapor, 3=İptal, 4=Bildirim.</summary>
    public int State { get; set; }
}

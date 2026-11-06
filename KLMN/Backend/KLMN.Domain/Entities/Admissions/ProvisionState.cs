using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Admissions;

/// <summary>Her provizyon durum değişikliği için tarihçeye eklenen kayıt.</summary>
public sealed class ProvisionState : BaseEntity
{
    public Guid ProvisionId { get; set; }
    public Provision Provision { get; set; } = null!;
    /// <summary>ProvisionStatusDefs sözlüğüne bağlı durum kodu.</summary>
    public int State { get; set; }
}

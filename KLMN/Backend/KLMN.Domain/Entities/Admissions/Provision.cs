using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Admissions;

/// <summary>Provizyon kaydı, sevk ve hastane ilişkileriyle birlikte yönetilir.</summary>
public sealed class Provision : BaseEntity
{
    public Guid DispatchId { get; set; }
    public Dispatch Dispatch { get; set; } = null!;
    public DateTime ProvisionDate { get; set; } = DateTime.UtcNow;
    public long ProvisionNo { get; set; }
    public Guid DispatchHospitalId { get; set; }
    public HospitalDef DispatchHospital { get; set; } = null!;
    public Guid? PhotoObjectId { get; set; }
    /// <summary>Sevksiz işlem işareti.</summary>
    public int? WithoutDispatch { get; set; }
    /// <summary>Durum kodu ProvisionStatusDefs sözlüğünden doğrulanır.</summary>
    public int CurrentState { get; set; } = ProvisionStatusCodes.New;
    public byte[]? Photo { get; set; }
    /// <summary>İlgili itiraz raporunun kimliği.</summary>
    public Guid? ObjectionableReportId { get; set; }
    public string? Description { get; set; }
    public ICollection<ProvisionState> States { get; set; } = new List<ProvisionState>();
}

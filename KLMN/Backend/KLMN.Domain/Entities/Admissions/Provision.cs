using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Admissions;

/// <summary>Eski MSKBS Provision kaydının GUID ve durum semantiği korunmuş karşılığı.</summary>
public sealed class Provision : BaseEntity
{
    public Guid DispatchId { get; set; }
    public Dispatch Dispatch { get; set; } = null!;
    public DateTime ProvisionDate { get; set; } = DateTime.UtcNow;
    public long ProvisionNo { get; set; }
    public Guid DispatchHospitalId { get; set; }
    public HospitalDef DispatchHospital { get; set; } = null!;
    public Guid? PhotoObjectId { get; set; }
    /// <summary>Eski modelde nullable int; 1=sevksiz, null/0=sevkli.</summary>
    public int? WithoutDispatch { get; set; }
    /// <summary>1=Yeni, 2=Rapor, 3=İptal, 4=Bildirim.</summary>
    public int CurrentState { get; set; } = 1;
    public byte[]? Photo { get; set; }
    /// <summary>Rapor modülündeki orijinal GUID. Report entity gelene kadar FK tanımlanmaz.</summary>
    public Guid? ObjectionableReportId { get; set; }
    public string? Description { get; set; }
    public ICollection<ProvisionState> States { get; set; } = new List<ProvisionState>();
}

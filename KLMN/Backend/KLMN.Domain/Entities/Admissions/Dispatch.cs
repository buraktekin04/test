using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Admissions;

/// <summary>Sevk Giriş kaydı; başvuru tanımlarıyla bağlantılıdır, audit ve xmin BaseEntity'dedir.</summary>
public sealed class Dispatch : BaseEntity
{
    public Guid PersonId { get; set; }
    public Person Person { get; set; } = null!;
    /// <summary>PostgreSQL identity ile üretilen, değiştirilemeyen sevk sıra numarasıdır.</summary>
    public long DispatchNo { get; set; }
    public DateTime DispatchDate { get; set; } = DateTime.UtcNow;
    /// <summary>1=Yeni, 2=Provizyon, 3=Rapor, 4=İptal, 5=Bildirim.</summary>
    public int CurrentState { get; set; } = 1;
    public int? NthAdmission { get; set; }
    public DateOnly? DispatchDocumentDate { get; set; }
    public string? DispatchDocumentNo { get; set; }
    public Guid ReasonForExaminationDefId { get; set; }
    /// <summary>1=Kapsam yok, 2=Şehir, 3=Sağlık tesisi.</summary>
    public int DispatchScope { get; set; } = 1;
    public Guid? DispatchScopeCityId { get; set; }
    public Guid? DispatchScopeHospitalId { get; set; }
    public Guid HcAdmissionTypeDefId { get; set; }
    public Guid? ForcesCommandDefId { get; set; }
    public Guid? RankDefId { get; set; }
    public Guid? MilitaryClassDefId { get; set; }
    public Guid? MilitaryUnitDefId { get; set; }
    public Guid? MilitarySchoolDefId { get; set; }
    /// <summary>Gönderen makam türüne göre bu iki FK'den yalnızca biri dolu olur.</summary>
    public Guid? SenderMilitaryUnitDefId { get; set; }
    public Guid? SenderMilitaryOfficeDefId { get; set; }
    public string? SenderChairOther { get; set; }
    public Guid? MilitaryOfficeDefId { get; set; }
    public Guid? FlyingMissionDefId { get; set; }
    public Guid? PlaneTypeDefId { get; set; }
    public string? RegistryNumber { get; set; }
    public DateOnly? LastNominationDate { get; set; }
    public string? MilitaryUnitDescription { get; set; }
    public int? PurchaseId { get; set; }
    public string? Description { get; set; }
    public bool AutomaticDispatch { get; set; }
    /// <summary>Başvurana ait fotoğraf; kimlik fotoğrafı belge eklerinden ayrı tutulur ve audit içine alınmaz.</summary>
    public byte[]? Photo { get; set; }
    /// <summary>Fotoğrafın doğrulanmış image/jpeg veya image/png medya türü.</summary>
    public string? PhotoContentType { get; set; }
    public ICollection<DispatchFile> Files { get; set; } = new List<DispatchFile>();
}

using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Admissions;

/// <summary>Sağlık tesisi: şehir ve isteğe bağlı hakem tesis ilişkileriyle birlikte tanımlanır.</summary>
public sealed class HospitalDef : EffectiveDatedEntity
{
    /// <summary>Silinmemiş kayıtlar arasında benzersiz sağlık tesisi kodu.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Sağlık tesisinin adı.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Şehir seçimi; eski CityDef nullable GUID alanıdır.</summary>
    public Guid? CityId { get; set; }
    public KLMN.Domain.Entities.Geography.CityDef? City { get; set; }
    /// <summary>Hakem sağlık tesisi; eski RefereeHospital nullable GUID alanıdır.</summary>
    public Guid? RefereeHospitalId { get; set; }
    public HospitalDef? RefereeHospital { get; set; }
    public ICollection<HospitalDef> ReferringHospitals { get; set; } = new List<HospitalDef>();
    public ICollection<HospitalReportType> ReportTypes { get; set; } = new List<HospitalReportType>();
}

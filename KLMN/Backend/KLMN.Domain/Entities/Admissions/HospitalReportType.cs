using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Admissions;

/// <summary>Sağlık tesisi ile rapor türünü ilişkilendiren kayıt; BaseEntity ortak audit ve xmin alanları.</summary>
public sealed class HospitalReportType : BaseEntity
{
    /// <summary>Sağlık tesisi ile rapor türü ilişkisi; ayrı Kod/Ad alanı bulunmaz.</summary>
    public Guid HospitalDefId { get; set; }
    public HospitalDef Hospital { get; set; } = null!;
    public Guid ReportTypeDefId { get; set; }
    public ReportTypeDef ReportType { get; set; } = null!;
}

using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>Muayene Sebebi Rapor Türleri; muayene sebebi ve mevcut referans kaydı arasındaki ilişki.</summary>
public sealed class ReasonForExamReportType : BaseEntity
{
    public Guid ReasonForExaminationDefId { get; set; }
    public ReasonForExaminationDef Reason { get; set; } = null!;
    public Guid ReportTypeDefId { get; set; }
    public ReportTypeDef ReportType { get; set; } = null!;
    
}

using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>Muayene Sebebi ana tanımı; audit, xmin ve geçerlilik tarihleri ortak katmanlardadır.</summary>
public sealed class ReasonForExaminationDef : EffectiveDatedEntity
{
    /// <summary>Muayene sebebinin benzersiz kodu.</summary>
    public string Code { get; set; } = string.Empty;
    /// <summary>Muayene sebebi açıklaması.</summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>Sevk gerekliliği.</summary>
    public bool DispatchRequired { get; set; }
    /// <summary>Sınıflandırma muayenesi olup olmadığı.</summary>
    public bool ClassificationExamination { get; set; }
    /// <summary>İtiraz edilebilir muayene sebebi.</summary>
    public bool Objectionable { get; set; }
    public ICollection<ReasonForExamReportType> ReportTypes { get; set; } = new List<ReasonForExamReportType>();
    public ICollection<ReasonForExamUnusedHcAdmissionType> UnusedAdmissionTypes { get; set; } = new List<ReasonForExamUnusedHcAdmissionType>();
    public ICollection<ReasonForExamProcedureDef> Procedures { get; set; } = new List<ReasonForExamProcedureDef>();
    public ICollection<ReasonForExamBranchDef> Branches { get; set; } = new List<ReasonForExamBranchDef>();
}

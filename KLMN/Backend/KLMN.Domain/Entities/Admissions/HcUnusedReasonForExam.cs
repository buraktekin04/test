using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>Kullanılmayacak Muayene Sebepleri: ana kurul kararıyla referans tanımı arasındaki aktif/pasif eşleştirme.</summary>
public sealed class HcUnusedReasonForExam:BaseEntity
{
    public Guid HcDecisionDefId { get; set; }
    public HcDecisionDef Decision { get; set; }=null!;
    public Guid ReasonForExaminationDefId { get; set; }
    public ReasonForExaminationDef Reason { get; set; }=null!;
}

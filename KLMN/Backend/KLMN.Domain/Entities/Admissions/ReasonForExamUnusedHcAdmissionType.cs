using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>Kullanılmayacak Hasta Grupları; muayene sebebi ve mevcut referans kaydı arasındaki ilişki.</summary>
public sealed class ReasonForExamUnusedHcAdmissionType : BaseEntity
{
    public Guid ReasonForExaminationDefId { get; set; }
    public ReasonForExaminationDef Reason { get; set; } = null!;
    public Guid HcAdmissionTypeDefId { get; set; }
    public HcAdmissionTypeDef AdmissionType { get; set; } = null!;
    
}

using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>Kullanılmayacak Başvuru Grupları: ana kurul kararıyla referans tanımı arasındaki aktif/pasif eşleştirme.</summary>
public sealed class HcUnusedAdmissionType:BaseEntity
{
    public Guid HcDecisionDefId { get; set; }
    public HcDecisionDef Decision { get; set; }=null!;
    public Guid HcAdmissionTypeDefId { get; set; }
    public HcAdmissionTypeDef AdmissionType { get; set; }=null!;
}

using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>Muayene Gereken Branşlar; muayene sebebi ve mevcut referans kaydı arasındaki ilişki.</summary>
public sealed class ReasonForExamBranchDef : BaseEntity
{
    public Guid ReasonForExaminationDefId { get; set; }
    public ReasonForExaminationDef Reason { get; set; } = null!;
    public Guid BranchDefId { get; set; }
    public BranchDef Branch { get; set; } = null!;
    /// <summary>Yaş ve cinsiyet koşulları eski kaynak entity'deki sayısal değerlerdir.</summary>
    public int StartingAge { get; set; }
    public int EndingAge { get; set; }
    public int Gender { get; set; }
}

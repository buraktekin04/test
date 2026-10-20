using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>Gereken Tetkikler; muayene sebebi ve mevcut referans kaydı arasındaki ilişki.</summary>
public sealed class ReasonForExamProcedureDef : BaseEntity
{
    public Guid ReasonForExaminationDefId { get; set; }
    public ReasonForExaminationDef Reason { get; set; } = null!;
    public Guid ProcedureDefId { get; set; }
    public ProcedureDef Procedure { get; set; } = null!;
    /// <summary>Yaş ve cinsiyet koşulları eski kaynak entity'deki sayısal değerlerdir.</summary>
    public int StartingAge { get; set; }
    public int EndingAge { get; set; }
    public int Gender { get; set; }
}

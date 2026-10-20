using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>Gereken Tetkikler eşleştirmeleri için asgari Tetkik Tanımı. Eski ProcedureDef alanları ayrıca doğrulanmalıdır.</summary>
public sealed class ProcedureDef : EffectiveDatedEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ICollection<ReasonForExamProcedureDef> ExaminationReasons { get; set; } = new List<ReasonForExamProcedureDef>();
}

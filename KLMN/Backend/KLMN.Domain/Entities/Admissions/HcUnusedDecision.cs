using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>Birlikte Verilmeyecek Kararlar: ana kurul kararıyla referans tanımı arasındaki aktif/pasif eşleştirme.</summary>
public sealed class HcUnusedDecision:BaseEntity
{
    public Guid HcDecisionDefId { get; set; }
    public HcDecisionDef Decision { get; set; }=null!;
    public Guid UnusedHcDecisionDefId { get; set; }
    public HcDecisionDef UnusedDecision { get; set; }=null!;
}

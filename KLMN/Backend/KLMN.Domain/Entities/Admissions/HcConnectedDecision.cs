using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>Bağlı Kararlar: ana kurul kararıyla referans tanımı arasındaki aktif/pasif eşleştirme.</summary>
public sealed class HcConnectedDecision:BaseEntity
{
    public Guid HcDecisionDefId { get; set; }
    public HcDecisionDef Decision { get; set; }=null!;
    public Guid HcConnectedDecisionDefId { get; set; }
    public HcConnectedDecisionDef ConnectedDecisionDef { get; set; }=null!;
}

using KLMN.Domain.Common;
using KLMN.Domain.Entities.MilitaryUnit;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>SYY tanısı, kurul kararı, başvuru grubu ve isteğe bağlı askerî sınıf için karar istatistiği.</summary>
public sealed class SYYDiagnosisDecision : BaseEntity
{
    /// <summary>SYY tanı tanımının kimliği.</summary>
    public Guid SYYDiagnosisDefId { get; set; }
    public SYYDiagnosisDef SYYDiagnosis { get; set; } = null!;
    /// <summary>Kurul kararı tanımının kimliği.</summary>
    public Guid HcDecisionDefId { get; set; }
    public HcDecisionDef HcDecision { get; set; } = null!;
    /// <summary>Başvuru grubunun kimliği.</summary>
    public Guid HcAdmissionTypeDefId { get; set; }
    public HcAdmissionTypeDef AdmissionType { get; set; } = null!;
    /// <summary>Askerî sınıf seçimi isteğe bağlıdır.</summary>
    public Guid? MilitaryClassDefId { get; set; }
    public MilitaryClassDef? MilitaryClass { get; set; }
    /// <summary>İlişkinin tanımlanmış istatistik adedi.</summary>
    public int Amount { get; set; }
}

using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>Kurul Karar Tanımları: karar kodu ve kararın iş kuralları. Audit, soft delete ve xmin ortak tabandadır.</summary>
public sealed class HcDecisionDef : EffectiveDatedEntity
{
    /// <summary>Eski MSKBS'deki sayısal karar kodu.</summary>
    public int Code { get; set; }
    /// <summary>Kararın görünen adı.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Karar için TimeInformationRequired iş kuralı.</summary>
    public bool TimeInformationRequired { get; set; }
    /// <summary>Karar için AdditionalDescriptionRequired iş kuralı.</summary>
    public bool AdditionalDescriptionRequired { get; set; }
    /// <summary>Karar için NegativeDecision iş kuralı.</summary>
    public bool NegativeDecision { get; set; }
    /// <summary>Karar için ApprovalRequired iş kuralı.</summary>
    public bool ApprovalRequired { get; set; }
    /// <summary>Karar için PertemApprovalRequired iş kuralı.</summary>
    public bool PertemApprovalRequired { get; set; }
    /// <summary>Karar için DiagnosisRequired iş kuralı.</summary>
    public bool DiagnosisRequired { get; set; }
    public ICollection<HcUnusedReasonForExam> UnusedReasons { get; set; } = new List<HcUnusedReasonForExam>();
    public ICollection<HcUnusedAdmissionType> UnusedAdmissionTypes { get; set; } = new List<HcUnusedAdmissionType>();
    public ICollection<HcUnusedDecision> UnusedDecisions { get; set; } = new List<HcUnusedDecision>();
    public ICollection<HcConnectedDecision> ConnectedDecisions { get; set; } = new List<HcConnectedDecision>();
}

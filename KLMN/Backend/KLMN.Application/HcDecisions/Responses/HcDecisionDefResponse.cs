using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.HcDecisions.Responses;
/// <summary>Kurul kararı ve bağlı aktif tanımların görüntüleme modeli; EF projeksiyonu Handler'dadır.</summary>
public sealed record HcDecisionDefResponse(Guid Id,int Code,string Name,bool TimeInformationRequired,bool AdditionalDescriptionRequired,bool NegativeDecision,bool ApprovalRequired,bool PertemApprovalRequired,bool DiagnosisRequired,DateTime? ValidFrom,DateTime? ValidTo,
 bool IsActive,uint Version,DateTime CreatedDate,IReadOnlyList<string> UnusedReasons,IReadOnlyList<string> UnusedAdmissionTypes,
 IReadOnlyList<string> UnusedDecisions,IReadOnlyList<string> ConnectedDecisions)
{
    /// <summary>İlgili sayfadaki önceden yüklenmiş FK adlarını ve karar alanlarını DTO'ya taşır.</summary>
    public static HcDecisionDefResponse From(HcDecisionDef x)=>new(x.Id,x.Code,x.Name,x.TimeInformationRequired,x.AdditionalDescriptionRequired,x.NegativeDecision,x.ApprovalRequired,x.PertemApprovalRequired,x.DiagnosisRequired,x.ValidFrom,x.ValidTo,
       x.IsActive,x.Version,x.CreatedDate,
       x.UnusedReasons.Where(y=>!y.IsDeleted&&y.IsActive).Select(y=>y.Reason.Description).ToArray(),
       x.UnusedAdmissionTypes.Where(y=>!y.IsDeleted&&y.IsActive).Select(y=>y.AdmissionType.Name).ToArray(),
       x.UnusedDecisions.Where(y=>!y.IsDeleted&&y.IsActive).Select(y=>y.UnusedDecision.Name).ToArray(),
       x.ConnectedDecisions.Where(y=>!y.IsDeleted&&y.IsActive).Select(y=>y.ConnectedDecisionDef.Name).ToArray());
}

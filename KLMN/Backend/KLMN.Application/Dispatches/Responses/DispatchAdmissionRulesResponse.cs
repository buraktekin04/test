namespace KLMN.Application.Dispatches.Responses;

/// <summary>Seçilen başvuru grubunun yeni sevkte istediği alanları tarif eder.</summary>
public sealed record DispatchAdmissionRulesResponse(
 bool ForcesCommandRequired,bool RankRequired,bool MilitaryClassRequired,bool MilitaryUnitRequired,
 bool SenderChairRequired,bool MilitaryOfficeRequired,bool RegistryNumberRequired,bool MilitarySchoolRequired,
 bool LastNominationDateRequired,bool NthAdmissionRequired,bool DispatchDocumentDateRequired,bool DispatchDocumentNoRequired,
 IReadOnlyList<Guid> UnusedReasonIds);

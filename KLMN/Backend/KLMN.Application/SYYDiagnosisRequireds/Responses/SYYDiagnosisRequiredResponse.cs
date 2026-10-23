namespace KLMN.Application.SYYDiagnosisRequireds.Responses;
public sealed record SYYDiagnosisRequiredResponse(Guid Id,Guid SYYDiagnosisDefId,
 string DiagnosisName,bool? MustAll,bool? MustOneOf,bool IsActive,uint Version,
 IReadOnlyList<RequiredDetailResponse> Details);

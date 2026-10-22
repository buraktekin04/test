namespace KLMN.Application.SYYDiagnosisRequireds.Responses;
/// <summary>Tek kurala ait birden fazla maddeyi aynı ekrana döndüren DTO.</summary>
public sealed record RequiredDetailResponse(Guid Id,string Description,int SortOrder);
public sealed record SYYDiagnosisRequiredResponse(Guid Id,Guid SYYDiagnosisDefId,
 string DiagnosisName,bool? MustAll,bool? MustOneOf,bool IsActive,uint Version,
 IReadOnlyList<RequiredDetailResponse> Details);
public sealed record SYYDiagnosisRequiredMutationResponse(Guid Id,uint Version);

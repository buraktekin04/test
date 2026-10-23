namespace KLMN.Application.SYYDiagnosisRequireds.Responses;
/// <summary>Tek kurala ait birden fazla maddeyi aynı ekrana döndüren DTO.</summary>
public sealed record RequiredDetailResponse(Guid Id,string Description,int SortOrder);

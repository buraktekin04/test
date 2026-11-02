using MediatR;

namespace KLMN.Application.ICD10Diagnostics.Queries.GetICD10DiagnosticOptions;

/// <summary>ICD10 tanımlarını tüm tabloyu yüklemeden, arayarak ve sınırlı sayıda döndürür.</summary>
public sealed record GetICD10DiagnosticCodeOptionsQuery(
 string? Search=null,Guid? SelectedICD10DefId=null):IRequest<IReadOnlyList<ICD10Option>>;

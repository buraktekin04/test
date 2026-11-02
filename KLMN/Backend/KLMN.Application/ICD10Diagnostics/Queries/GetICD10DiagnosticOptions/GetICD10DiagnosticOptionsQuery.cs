using MediatR;
using System.Text.Json.Serialization;
namespace KLMN.Application.ICD10Diagnostics.Queries.GetICD10DiagnosticOptions;
/// <summary>Seçili pasif tarihsel kayıtlar dahil bağımsız istatistik dropdown verileri.</summary>
public sealed record ICD10Option(Guid Id,string Label,bool IsActive);
/// <summary>
/// ASP.NET Core camelCase may serialize ICD10s as icD10s (acronym followed by number).
/// Explicit JSON name keeps the frontend icd10s contract stable.
/// </summary>
public sealed record ICD10DiagnosticOptions(
 [property: JsonPropertyName("icd10s")] IReadOnlyList<ICD10Option> ICD10s,
 [property: JsonPropertyName("syyDiagnoses")] IReadOnlyList<ICD10Option> SYYDiagnoses,
 [property: JsonPropertyName("admissionTypes")] IReadOnlyList<ICD10Option> AdmissionTypes);
public sealed record GetICD10DiagnosticOptionsQuery(Guid? SelectedICD10DefId=null,Guid? SelectedSYYDiagnosisDefId=null,
 Guid? SelectedHcAdmissionTypeDefId=null):IRequest<ICD10DiagnosticOptions>;

using MediatR;
namespace KLMN.Application.ICD10Diagnostics.Queries.GetICD10DiagnosticOptions;
/// <summary>Seçili pasif tarihsel kayıtlar dahil bağımsız istatistik dropdown verileri.</summary>
public sealed record ICD10Option(Guid Id,string Label,bool IsActive);
public sealed record ICD10DiagnosticOptions(
 IReadOnlyList<ICD10Option> ICD10s,IReadOnlyList<ICD10Option> SYYDiagnoses,
 IReadOnlyList<ICD10Option> AdmissionTypes);
public sealed record GetICD10DiagnosticOptionsQuery(Guid? SelectedICD10DefId=null,Guid? SelectedSYYDiagnosisDefId=null,
 Guid? SelectedHcAdmissionTypeDefId=null):IRequest<ICD10DiagnosticOptions>;

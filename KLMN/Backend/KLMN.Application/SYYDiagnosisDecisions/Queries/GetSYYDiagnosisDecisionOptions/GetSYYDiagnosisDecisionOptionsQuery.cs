using KLMN.Application.SYYDiagnosisDecisions.Responses;
using MediatR;
namespace KLMN.Application.SYYDiagnosisDecisions.Queries.GetSYYDiagnosisDecisionOptions;
/// <summary>Tanımlama için seçili eski pasif FK'leri de getirebilir; sorgulama seçimlerinde aktifler.</summary>
public sealed record GetSYYDiagnosisDecisionOptionsQuery(
 Guid? SelectedSYYDiagnosisDefId=null,Guid? SelectedHcDecisionDefId=null,
 Guid? SelectedHcAdmissionTypeDefId=null,Guid? SelectedMilitaryClassDefId=null,bool IncludeInactive=false)
 :IRequest<SYYDiagnosisDecisionOptionsResponse>;

using MediatR;
using KLMN.Application.SYYDiagnosisDecisions.Responses;
namespace KLMN.Application.SYYDiagnosisDecisions.Commands.UpdateSYYDiagnosisDecision;
/// <summary>Update işlem isteği; handler ve validator ayrı dosyalardır.</summary>
public sealed record UpdateSYYDiagnosisDecisionCommand(Guid Id,uint Version,Guid SYYDiagnosisDefId,Guid HcDecisionDefId,Guid HcAdmissionTypeDefId,Guid? MilitaryClassDefId,int Amount):IRequest<SYYDiagnosisDecisionMutationResponse>;

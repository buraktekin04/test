using MediatR;
using KLMN.Application.SYYDiagnosisDecisions.Responses;
namespace KLMN.Application.SYYDiagnosisDecisions.Commands.CreateSYYDiagnosisDecision;
/// <summary>Create işlem isteği; handler ve validator ayrı dosyalardır.</summary>
public sealed record CreateSYYDiagnosisDecisionCommand(Guid SYYDiagnosisDefId,Guid HcDecisionDefId,Guid HcAdmissionTypeDefId,Guid? MilitaryClassDefId,int Amount):IRequest<SYYDiagnosisDecisionMutationResponse>;

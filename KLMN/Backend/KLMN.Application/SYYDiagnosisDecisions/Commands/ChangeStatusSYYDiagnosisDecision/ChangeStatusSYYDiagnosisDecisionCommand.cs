using MediatR;
using KLMN.Application.SYYDiagnosisDecisions.Responses;
namespace KLMN.Application.SYYDiagnosisDecisions.Commands.ChangeStatusSYYDiagnosisDecision;
/// <summary>ChangeStatus işlem isteği; handler ve validator ayrı dosyalardır.</summary>
public sealed record ChangeStatusSYYDiagnosisDecisionCommand(Guid Id,uint Version,bool IsActive):IRequest<SYYDiagnosisDecisionMutationResponse>;

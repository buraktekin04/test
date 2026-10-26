using MediatR;

namespace KLMN.Application.SYYDiagnosisDecisions.Commands.DeleteSYYDiagnosisDecision;
/// <summary>Delete işlem isteği; handler ve validator ayrı dosyalardır.</summary>
public sealed record DeleteSYYDiagnosisDecisionCommand(Guid Id,uint Version):IRequest;

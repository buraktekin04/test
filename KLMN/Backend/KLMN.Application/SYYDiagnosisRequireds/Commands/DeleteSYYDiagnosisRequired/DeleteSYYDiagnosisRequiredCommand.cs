using MediatR;

namespace KLMN.Application.SYYDiagnosisRequireds.Commands.DeleteSYYDiagnosisRequired;
public sealed record DeleteSYYDiagnosisRequiredCommand(Guid Id,uint Version):IRequest;

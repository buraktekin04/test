using MediatR;
using KLMN.Application.SYYDiagnosisRequireds.Responses;
namespace KLMN.Application.SYYDiagnosisRequireds.Commands.ChangeStatusSYYDiagnosisRequired;
public sealed record ChangeStatusSYYDiagnosisRequiredCommand(Guid Id,uint Version,bool IsActive):IRequest<SYYDiagnosisRequiredMutationResponse>;

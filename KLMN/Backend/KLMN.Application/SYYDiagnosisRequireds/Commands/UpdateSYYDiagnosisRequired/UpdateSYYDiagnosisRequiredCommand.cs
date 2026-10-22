using MediatR;
using KLMN.Application.SYYDiagnosisRequireds.Responses;
namespace KLMN.Application.SYYDiagnosisRequireds.Commands.UpdateSYYDiagnosisRequired;
public sealed record UpdateSYYDiagnosisRequiredCommand(Guid Id,uint Version,Guid SYYDiagnosisDefId,bool? MustAll,bool? MustOneOf,IReadOnlyList<string> Descriptions):IRequest<SYYDiagnosisRequiredMutationResponse>;

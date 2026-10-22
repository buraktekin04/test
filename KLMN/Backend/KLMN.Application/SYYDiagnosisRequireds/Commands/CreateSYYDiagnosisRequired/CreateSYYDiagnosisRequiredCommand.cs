using MediatR;
using KLMN.Application.SYYDiagnosisRequireds.Responses;
namespace KLMN.Application.SYYDiagnosisRequireds.Commands.CreateSYYDiagnosisRequired;
public sealed record CreateSYYDiagnosisRequiredCommand(Guid SYYDiagnosisDefId,bool? MustAll,bool? MustOneOf,IReadOnlyList<string> Descriptions):IRequest<SYYDiagnosisRequiredMutationResponse>;

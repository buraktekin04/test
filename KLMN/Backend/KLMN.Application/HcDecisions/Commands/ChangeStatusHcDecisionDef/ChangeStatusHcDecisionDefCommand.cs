using MediatR;
using KLMN.Application.HcDecisions.Responses;
namespace KLMN.Application.HcDecisions.Commands.ChangeStatusHcDecisionDef;
/// <summary>Kurul kararı ChangeStatus CQRS isteği.</summary>
public sealed record ChangeStatusHcDecisionDefCommand(Guid Id,uint Version,bool IsActive):IRequest<HcDecisionDefMutationResponse>;

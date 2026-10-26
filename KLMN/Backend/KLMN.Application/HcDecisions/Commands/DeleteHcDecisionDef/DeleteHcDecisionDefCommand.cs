using MediatR;

namespace KLMN.Application.HcDecisions.Commands.DeleteHcDecisionDef;
/// <summary>Kurul kararı Delete CQRS isteği.</summary>
public sealed record DeleteHcDecisionDefCommand(Guid Id,uint Version):IRequest;

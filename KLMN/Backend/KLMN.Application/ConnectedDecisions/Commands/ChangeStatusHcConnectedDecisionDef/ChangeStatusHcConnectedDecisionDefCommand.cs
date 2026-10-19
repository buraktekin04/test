using KLMN.Application.ConnectedDecisions.Responses;
using MediatR;

namespace KLMN.Application.ConnectedDecisions.Commands.ChangeStatusHcConnectedDecisionDef;

/// <summary>Bağlı kurul kararı ChangeStatus CQRS komutu.</summary>
public sealed record ChangeStatusHcConnectedDecisionDefCommand(Guid Id, uint Version, bool IsActive) : IRequest<HcConnectedDecisionDefMutationResponse>;

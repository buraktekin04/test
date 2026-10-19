using KLMN.Application.ConnectedDecisions.Responses;
using MediatR;

namespace KLMN.Application.ConnectedDecisions.Commands.UpdateHcConnectedDecisionDef;

/// <summary>Bağlı kurul kararı Update CQRS komutu.</summary>
public sealed record UpdateHcConnectedDecisionDefCommand(Guid Id, uint Version, string Code, string Name, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<HcConnectedDecisionDefMutationResponse>;

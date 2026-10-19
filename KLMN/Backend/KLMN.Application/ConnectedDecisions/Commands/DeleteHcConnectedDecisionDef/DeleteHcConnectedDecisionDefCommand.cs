using MediatR;

namespace KLMN.Application.ConnectedDecisions.Commands.DeleteHcConnectedDecisionDef;

/// <summary>Bağlı kurul kararı Delete CQRS komutu.</summary>
public sealed record DeleteHcConnectedDecisionDefCommand(Guid Id, uint Version) : IRequest;

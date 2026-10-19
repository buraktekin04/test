using KLMN.Application.ConnectedDecisions.Responses;
using MediatR;

namespace KLMN.Application.ConnectedDecisions.Commands.CreateHcConnectedDecisionDef;

/// <summary>Bağlı kurul kararı Create CQRS komutu.</summary>
public sealed record CreateHcConnectedDecisionDefCommand(string Code, string Name, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<HcConnectedDecisionDefMutationResponse>;

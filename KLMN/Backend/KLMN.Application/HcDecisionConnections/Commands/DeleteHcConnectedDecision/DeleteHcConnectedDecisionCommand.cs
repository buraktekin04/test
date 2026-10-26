using MediatR;

namespace KLMN.Application.HcDecisionConnections.Commands.DeleteHcConnectedDecision;
/// <summary>Bağlı Kararlar için bağımsız Delete isteği.</summary>
public sealed record DeleteHcConnectedDecisionCommand(Guid Id,uint Version):IRequest;

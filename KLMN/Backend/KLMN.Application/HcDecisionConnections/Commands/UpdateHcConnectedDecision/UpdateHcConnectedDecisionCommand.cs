using MediatR;
using KLMN.Application.HcDecisionConnections.Responses;
namespace KLMN.Application.HcDecisionConnections.Commands.UpdateHcConnectedDecision;
/// <summary>Bağlı Kararlar için bağımsız Update isteği.</summary>
public sealed record UpdateHcConnectedDecisionCommand(Guid Id,uint Version,Guid HcDecisionDefId,Guid ReferenceId):IRequest<HcConnectedDecisionMutationResponse>;

using MediatR;
using KLMN.Application.HcDecisionConnections.Responses;
namespace KLMN.Application.HcDecisionConnections.Commands.CreateHcConnectedDecision;
/// <summary>Bağlı Kararlar için bağımsız Create isteği.</summary>
public sealed record CreateHcConnectedDecisionCommand(Guid HcDecisionDefId,Guid ReferenceId):IRequest<HcConnectedDecisionMutationResponse>;

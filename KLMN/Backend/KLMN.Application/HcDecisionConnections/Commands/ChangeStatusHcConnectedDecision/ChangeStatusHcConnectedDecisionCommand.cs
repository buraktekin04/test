using MediatR;
using KLMN.Application.HcDecisionConnections.Responses;
namespace KLMN.Application.HcDecisionConnections.Commands.ChangeStatusHcConnectedDecision;
/// <summary>Bağlı Kararlar için bağımsız ChangeStatus isteği.</summary>
public sealed record ChangeStatusHcConnectedDecisionCommand(Guid Id,uint Version,bool IsActive):IRequest<HcConnectedDecisionMutationResponse>;

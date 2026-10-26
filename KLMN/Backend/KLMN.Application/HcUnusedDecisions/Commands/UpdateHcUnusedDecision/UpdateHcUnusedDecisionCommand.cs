using MediatR;
using KLMN.Application.HcUnusedDecisions.Responses;
namespace KLMN.Application.HcUnusedDecisions.Commands.UpdateHcUnusedDecision;
/// <summary>Birlikte Verilmeyecek Kararlar için bağımsız Update isteği.</summary>
public sealed record UpdateHcUnusedDecisionCommand(Guid Id,uint Version,Guid HcDecisionDefId,Guid ReferenceId):IRequest<HcUnusedDecisionMutationResponse>;

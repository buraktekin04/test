using MediatR;
using KLMN.Application.HcUnusedDecisions.Responses;
namespace KLMN.Application.HcUnusedDecisions.Commands.CreateHcUnusedDecision;
/// <summary>Birlikte Verilmeyecek Kararlar için bağımsız Create isteği.</summary>
public sealed record CreateHcUnusedDecisionCommand(Guid HcDecisionDefId,Guid ReferenceId):IRequest<HcUnusedDecisionMutationResponse>;

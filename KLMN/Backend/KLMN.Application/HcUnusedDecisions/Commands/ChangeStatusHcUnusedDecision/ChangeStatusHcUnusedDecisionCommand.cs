using MediatR;
using KLMN.Application.HcUnusedDecisions.Responses;
namespace KLMN.Application.HcUnusedDecisions.Commands.ChangeStatusHcUnusedDecision;
/// <summary>Birlikte Verilmeyecek Kararlar için bağımsız ChangeStatus isteği.</summary>
public sealed record ChangeStatusHcUnusedDecisionCommand(Guid Id,uint Version,bool IsActive):IRequest<HcUnusedDecisionMutationResponse>;

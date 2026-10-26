using MediatR;

namespace KLMN.Application.HcUnusedDecisions.Commands.DeleteHcUnusedDecision;
/// <summary>Birlikte Verilmeyecek Kararlar için bağımsız Delete isteği.</summary>
public sealed record DeleteHcUnusedDecisionCommand(Guid Id,uint Version):IRequest;

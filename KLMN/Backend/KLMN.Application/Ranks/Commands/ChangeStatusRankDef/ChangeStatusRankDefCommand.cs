using KLMN.Application.Ranks.Responses;
using MediatR;

namespace KLMN.Application.Ranks.Commands.ChangeStatusRankDef;

/// <summary>Rütbe ChangeStatus CQRS komutu.</summary>
public sealed record ChangeStatusRankDefCommand(Guid Id, uint Version, bool IsActive) : IRequest<RankDefMutationResponse>;

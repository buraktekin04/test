using KLMN.Application.RankGroups.Responses;
using MediatR;

namespace KLMN.Application.RankGroups.Commands.ChangeStatusRankGroupDef;

/// <summary>Rütbe Grubu ChangeStatus CQRS komutu.</summary>
public sealed record ChangeStatusRankGroupDefCommand(Guid Id, uint Version, bool IsActive) : IRequest<RankGroupDefMutationResponse>;

using MediatR;

namespace KLMN.Application.RankGroups.Commands.DeleteRankGroupDef;

/// <summary>Rütbe Grubu Delete CQRS komutu.</summary>
public sealed record DeleteRankGroupDefCommand(Guid Id, uint Version) : IRequest;

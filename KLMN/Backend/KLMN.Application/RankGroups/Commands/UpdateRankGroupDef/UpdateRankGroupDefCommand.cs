using KLMN.Application.RankGroups.Responses;
using MediatR;

namespace KLMN.Application.RankGroups.Commands.UpdateRankGroupDef;

/// <summary>Rütbe Grubu Update CQRS komutu.</summary>
public sealed record UpdateRankGroupDefCommand(Guid Id, uint Version, string Code, string Name, string? ShortName, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<RankGroupDefMutationResponse>;

using KLMN.Application.Ranks.Responses;
using MediatR;

namespace KLMN.Application.Ranks.Commands.UpdateRankDef;

/// <summary>Rütbe Update CQRS komutu.</summary>
public sealed record UpdateRankDefCommand(Guid Id, uint Version, string Code, string Name, string? ShortName, int Type, Guid? RankGroupId, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<RankDefMutationResponse>;

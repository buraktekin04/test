using KLMN.Application.Ranks.Responses;
using MediatR;

namespace KLMN.Application.Ranks.Commands.CreateRankDef;

/// <summary>Rütbe Create CQRS komutu.</summary>
public sealed record CreateRankDefCommand(string Code, string Name, string? ShortName, int Type, Guid? RankGroupId, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<RankDefMutationResponse>;

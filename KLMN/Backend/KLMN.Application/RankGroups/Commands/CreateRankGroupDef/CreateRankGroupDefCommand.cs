using KLMN.Application.RankGroups.Responses;
using MediatR;

namespace KLMN.Application.RankGroups.Commands.CreateRankGroupDef;

/// <summary>Rütbe Grubu Create CQRS komutu.</summary>
public sealed record CreateRankGroupDefCommand(string Code, string Name, string? ShortName, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<RankGroupDefMutationResponse>;

using MediatR;

namespace KLMN.Application.Ranks.Commands.DeleteRankDef;

/// <summary>Rütbe Delete CQRS komutu.</summary>
public sealed record DeleteRankDefCommand(Guid Id, uint Version) : IRequest;

using MediatR;
using KLMN.Application.ForcesCommands.Responses;

namespace KLMN.Application.ForcesCommands.Commands.ChangeForcesCommandDefStatus;

/// <summary>ChangeForcesCommandDefStatus işleminin bağımsız CQRS komut sözleşmesidir.</summary>
public sealed record ChangeForcesCommandDefStatusCommand(Guid Id, uint Version, bool IsActive) : IRequest<ForcesCommandDefMutationResponse>;

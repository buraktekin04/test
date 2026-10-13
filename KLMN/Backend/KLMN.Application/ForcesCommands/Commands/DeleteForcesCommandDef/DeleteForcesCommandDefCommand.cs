using MediatR;

namespace KLMN.Application.ForcesCommands.Commands.DeleteForcesCommandDef;

/// <summary>DeleteForcesCommandDef işleminin bağımsız CQRS komut sözleşmesidir.</summary>
public sealed record DeleteForcesCommandDefCommand(Guid Id, uint Version) : IRequest;

using MediatR;
using KLMN.Application.ForcesCommands.Responses;

namespace KLMN.Application.ForcesCommands.Commands.UpdateForcesCommandDef;

/// <summary>UpdateForcesCommandDef işleminin bağımsız CQRS komut sözleşmesidir.</summary>
public sealed record UpdateForcesCommandDefCommand(Guid Id, uint Version, string Name, string Code, DateTime? ValidFrom = null, DateTime? ValidTo = null) : IRequest<ForcesCommandDefMutationResponse>;

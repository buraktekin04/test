using MediatR;
using KLMN.Application.ForcesCommands.Responses;

namespace KLMN.Application.ForcesCommands.Commands.CreateForcesCommandDef;

/// <summary>CreateForcesCommandDef işleminin bağımsız CQRS komut sözleşmesidir.</summary>
public sealed record CreateForcesCommandDefCommand(string Name, string Code, DateTime? ValidFrom = null, DateTime? ValidTo = null) : IRequest<ForcesCommandDefMutationResponse>;

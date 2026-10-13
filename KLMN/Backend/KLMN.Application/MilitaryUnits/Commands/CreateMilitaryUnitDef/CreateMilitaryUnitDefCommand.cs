using MediatR;
using KLMN.Application.MilitaryUnits.Responses;

namespace KLMN.Application.MilitaryUnits.Commands.CreateMilitaryUnitDef;

/// <summary>CreateMilitaryUnitDef işleminin bağımsız CQRS komut sözleşmesidir.</summary>
public sealed record CreateMilitaryUnitDefCommand(string Name, string Code, Guid? ConnectedTopUnity, string? ExternalCode = null, Guid? ForcesCommandDef = null, DateTime? ValidFrom = null, DateTime? ValidTo = null) : IRequest<MilitaryUnitDefMutationResponse>;

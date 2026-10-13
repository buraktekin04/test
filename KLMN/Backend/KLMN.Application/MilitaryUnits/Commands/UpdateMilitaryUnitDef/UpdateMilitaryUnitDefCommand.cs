using MediatR;
using KLMN.Application.MilitaryUnits.Responses;

namespace KLMN.Application.MilitaryUnits.Commands.UpdateMilitaryUnitDef;

/// <summary>UpdateMilitaryUnitDef işleminin bağımsız CQRS komut sözleşmesidir.</summary>
public sealed record UpdateMilitaryUnitDefCommand(Guid Id, uint Version, string Name, string Code, Guid? ConnectedTopUnity, string? ExternalCode = null, Guid? ForcesCommandDef = null, DateTime? ValidFrom = null, DateTime? ValidTo = null) : IRequest<MilitaryUnitDefMutationResponse>;

using MediatR;

namespace KLMN.Application.MilitaryUnits.Commands.DeleteMilitaryUnitDef;

/// <summary>DeleteMilitaryUnitDef işleminin bağımsız CQRS komut sözleşmesidir.</summary>
public sealed record DeleteMilitaryUnitDefCommand(Guid Id, uint Version) : IRequest;

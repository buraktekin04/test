using MediatR;
using KLMN.Application.MilitaryUnits.Responses;

namespace KLMN.Application.MilitaryUnits.Commands.ChangeMilitaryUnitDefStatus;

/// <summary>ChangeMilitaryUnitDefStatus işleminin bağımsız CQRS komut sözleşmesidir.</summary>
public sealed record ChangeMilitaryUnitDefStatusCommand(Guid Id, uint Version, bool IsActive) : IRequest<MilitaryUnitDefMutationResponse>;

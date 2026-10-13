using KLMN.Application.MilitaryUnits.Responses;
using MediatR;

namespace KLMN.Application.MilitaryUnits.Queries.GetForceCommandOptions;

public sealed record GetForceCommandOptionsQuery : IRequest<IReadOnlyList<ForceCommandOptionResponse>>;

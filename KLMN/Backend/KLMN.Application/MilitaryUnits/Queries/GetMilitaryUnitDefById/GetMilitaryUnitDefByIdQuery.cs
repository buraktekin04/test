using KLMN.Application.MilitaryUnits.Responses;
using MediatR;

namespace KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitDefById;

/// <summary>Güncel birlik kaydını ve optimistic concurrency versiyonunu ister.</summary>
public sealed record GetMilitaryUnitDefByIdQuery(Guid Id) : IRequest<MilitaryUnitDefResponse>;

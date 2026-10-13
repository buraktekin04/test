using KLMN.Application.MilitaryUnits.Responses;
using MediatR;

namespace KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitParentOptions;

/// <summary>Üst birim seçiminde kullanılabilen aktif birlik kayıtlarını ister.</summary>
public sealed record GetMilitaryUnitParentOptionsQuery()
    : IRequest<IReadOnlyList<MilitaryUnitParentOptionResponse>>;

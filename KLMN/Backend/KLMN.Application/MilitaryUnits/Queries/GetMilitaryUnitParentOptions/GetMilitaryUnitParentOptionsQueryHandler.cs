using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryUnits.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitParentOptions;

/// <summary>Birlik seçicisinde yalnızca aktif birimleri gösterir.</summary>
public sealed class GetMilitaryUnitParentOptionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryUnitParentOptionsQuery, IReadOnlyList<MilitaryUnitParentOptionResponse>>
{
    public async Task<IReadOnlyList<MilitaryUnitParentOptionResponse>> Handle(
        GetMilitaryUnitParentOptionsQuery request, CancellationToken ct) =>
        await db.MilitaryUnitDefs.AsNoTracking()
            .OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Select(x => new MilitaryUnitParentOptionResponse(
                x.Id, x.Code, x.Name, x.ConnectedTopUnity))
            .ToListAsync(ct);
}

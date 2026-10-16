using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryUnits.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitParentOptions;

/// <summary>Varsayılan aktif üst birlikleri veya filtre için tüm silinmemiş üst birlikleri getirir.</summary>
public sealed class GetMilitaryUnitParentOptionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryUnitParentOptionsQuery, IReadOnlyList<MilitaryUnitParentOptionResponse>>
{
    public async Task<IReadOnlyList<MilitaryUnitParentOptionResponse>> Handle(
        GetMilitaryUnitParentOptionsQuery request, CancellationToken ct)
    {
        var query = request.IncludeInactive
            ? db.MilitaryUnitDefs.IgnoreQueryFilters().AsNoTracking().Where(x => !x.IsDeleted)
            : db.MilitaryUnitDefs.AsNoTracking();

        return await query.OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Select(x => new MilitaryUnitParentOptionResponse(
                x.Id, x.Code, x.Name, x.ConnectedTopUnity,
                x.ForceCommand != null ? x.ForceCommand.Name : null))
            .ToListAsync(ct);
    }
}

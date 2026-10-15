using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitFilterOptions;

/// <summary>
/// Silinmemiş kuvvetleri ve üst birlikleri aktifliklerinden bağımsız getirir;
/// geçmişteki pasif bağlar da listeden sorgulanabilmelidir.
/// </summary>
public sealed class GetMilitaryUnitFilterOptionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryUnitFilterOptionsQuery, MilitaryUnitFilterOptionsResponse>
{
    /// <summary>Yalnızca kimlik ve görüntüleme alanlarını döndürür.</summary>
    public async Task<MilitaryUnitFilterOptionsResponse> Handle(
        GetMilitaryUnitFilterOptionsQuery request, CancellationToken ct)
    {
        var forces = await db.ForcesCommandDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Select(x => new MilitaryUnitFilterOption(x.Id, x.Code, x.Name, null))
            .ToListAsync(ct);

        var parents = await db.MilitaryUnitDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Select(x => new MilitaryUnitFilterOption(x.Id, x.Code, x.Name,
                x.ForceCommand != null ? x.ForceCommand.Name : null))
            .ToListAsync(ct);

        return new MilitaryUnitFilterOptionsResponse(forces, parents);
    }
}

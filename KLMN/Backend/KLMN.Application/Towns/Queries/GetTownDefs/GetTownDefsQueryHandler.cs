using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.Towns.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Towns.Queries.GetTownDefs;
/// <summary>İlçe tanımına ait sorgu, yanıt veya iş kuralı bileşeni.</summary>
public sealed class GetTownDefsQueryHandler(IKLMNDbContext db) : IRequestHandler<GetTownDefsQuery, PagedResult<TownDefResponse>>
{
    public async Task<PagedResult<TownDefResponse>> Handle(GetTownDefsQuery request, CancellationToken ct)
    {
        var query = db.TownDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (request.IncludeInactive || request.IsActive == false || x.IsActive));
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.Code.ToUpper().Contains(search) ||
                x.Name.ToUpper().Contains(search) ||
                (x.ExternalCode != null && x.ExternalCode.ToUpper().Contains(search))
                || (x.City != null && x.City.Name.ToUpper().Contains(search)));
        }
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive);
        if (request.CityId.HasValue) query = query.Where(x => x.CityId == request.CityId);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(TownDefResponse.Projection).ToListAsync(ct);
        return new PagedResult<TownDefResponse>
        { Items=items, TotalCount=total, PageNumber=request.PageNumber, PageSize=request.PageSize };
    }
}

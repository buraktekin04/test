using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.Cities.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Cities.Queries.GetCityDefs;
public sealed class GetCityDefsQueryHandler(IKLMNDbContext db) : IRequestHandler<GetCityDefsQuery, PagedResult<CityDefResponse>>
{
    public async Task<PagedResult<CityDefResponse>> Handle(GetCityDefsQuery request, CancellationToken ct)
    {
        var query = db.CityDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (request.IncludeInactive || request.IsActive == false || x.IsActive));
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.Code.ToUpper().Contains(search) ||
                x.Name.ToUpper().Contains(search) ||
                (x.ExternalCode != null && x.ExternalCode.ToUpper().Contains(search))
                || (x.Country != null && x.Country.Name.ToUpper().Contains(search)));
        }
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive);
        if (request.CountryId.HasValue) query = query.Where(x => x.CountryId == request.CountryId);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(CityDefResponse.Projection).ToListAsync(ct);
        return new PagedResult<CityDefResponse>
        { Items=items, TotalCount=total, PageNumber=request.PageNumber, PageSize=request.PageSize };
    }
}

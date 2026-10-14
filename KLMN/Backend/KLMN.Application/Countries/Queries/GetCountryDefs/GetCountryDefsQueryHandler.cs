using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.Countries.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Countries.Queries.GetCountryDefs;
public sealed class GetCountryDefsQueryHandler(IKLMNDbContext db) : IRequestHandler<GetCountryDefsQuery, PagedResult<CountryDefResponse>>
{
    public async Task<PagedResult<CountryDefResponse>> Handle(GetCountryDefsQuery request, CancellationToken ct)
    {
        var query = db.CountryDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (request.IncludeInactive || request.IsActive == false || x.IsActive));
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.Code.ToUpper().Contains(search) ||
                x.Name.ToUpper().Contains(search) ||
                (x.ExternalCode != null && x.ExternalCode.ToUpper().Contains(search))
                );
        }
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive);
        if (request.IsTownRequired.HasValue) query = query.Where(x => x.IsTownRequired == request.IsTownRequired);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(CountryDefResponse.Projection).ToListAsync(ct);
        return new PagedResult<CountryDefResponse>
        { Items=items, TotalCount=total, PageNumber=request.PageNumber, PageSize=request.PageSize };
    }
}

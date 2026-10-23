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
                (x.ExternalCode != null && x.ExternalCode.ToUpper().Contains(search)) ||
                (x.ShortCode != null && x.ShortCode.ToUpper().Contains(search)));
        }
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive);
        if (request.IsTownRequired.HasValue) query = query.Where(x => x.IsTownRequired == request.IsTownRequired);
        if (!string.IsNullOrWhiteSpace(request.ShortCode))
        {
            var shortCode = request.ShortCode.Trim().ToUpperInvariant();
            query = query.Where(x => x.ShortCode != null && x.ShortCode.ToUpper().Contains(shortCode));
        }
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(x =>
        new CountryDefResponse
        {
            Id=x.Id, Code=x.Code, Name=x.Name, ExternalCode=x.ExternalCode, ShortCode=x.ShortCode,
            IsTownRequired=x.IsTownRequired, CityCount=x.Cities.Count(c => !c.IsDeleted),
            IsActive=x.IsActive, CreatedDate=x.CreatedDate, CreatedBy=x.CreatedBy,
            UpdatedDate=x.UpdatedDate, UpdatedBy=x.UpdatedBy, Version=x.Version
        }).ToListAsync(ct);
        return new PagedResult<CountryDefResponse>
        { Items=items, TotalCount=total, PageNumber=request.PageNumber, PageSize=request.PageSize };
    }
}

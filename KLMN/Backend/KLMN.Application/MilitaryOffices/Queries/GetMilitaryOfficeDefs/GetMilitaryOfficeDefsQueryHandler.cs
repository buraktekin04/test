using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.MilitaryOffices.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryOffices.Queries.GetMilitaryOfficeDefs;

/// <summary>Sunucuda filtrelenen askerlik şubelerini sayfalı DTO olarak döndürür.</summary>
public sealed class GetMilitaryOfficeDefsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryOfficeDefsQuery, PagedResult<MilitaryOfficeDefResponse>>
{
    /// <summary>Aktif/pasif, şehir ve aramayı SQL'e uygular.</summary>
    public async Task<PagedResult<MilitaryOfficeDefResponse>> Handle(
        GetMilitaryOfficeDefsQuery request, CancellationToken ct)
    {
        var query = db.MilitaryOfficeDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (request.IncludeInactive || request.IsActive == false || x.IsActive));
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.Code.ToUpper().Contains(search)
                || x.Name.ToUpper().Contains(search)
                || (x.ExternalCode != null && x.ExternalCode.ToUpper().Contains(search))
                || (x.City != null && x.City.Name.ToUpper().Contains(search)));
        }
        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive);
        if (request.CityId.HasValue)
            query = query.Where(x => x.CityId == request.CityId);
        var count = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(MilitaryOfficeDefResponse.Projection).ToListAsync(ct);
        return new PagedResult<MilitaryOfficeDefResponse>
        { Items = items, TotalCount = count, PageNumber = request.PageNumber, PageSize = request.PageSize };
    }
}

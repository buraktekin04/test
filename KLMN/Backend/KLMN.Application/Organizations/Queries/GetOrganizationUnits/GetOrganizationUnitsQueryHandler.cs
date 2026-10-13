using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.Organizations.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Organizations.Queries.GetOrganizationUnits;

/// <summary>Aktif/pasif dahil filtreleri tüm veritabanına uygular ve sayfalar.</summary>
public sealed class GetOrganizationUnitsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetOrganizationUnitsQuery, PagedResult<OrganizationUnitResponse>>
{
    public async Task<PagedResult<OrganizationUnitResponse>> Handle(
        GetOrganizationUnitsQuery request, CancellationToken ct)
    {
        var query = request.IncludeInactive || request.IsActive == false
            ? db.OrganizationUnits.IgnoreQueryFilters().Where(x => !x.IsDeleted)
            : db.OrganizationUnits.AsQueryable();

        query = query.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.Code.ToUpper().Contains(search) ||
                x.Name.ToUpper().Contains(search) ||
                (x.ParentOrganizationUnit != null &&
                    (x.ParentOrganizationUnit.Name.ToUpper().Contains(search) ||
                     x.ParentOrganizationUnit.Code.ToUpper().Contains(search))));
        }
        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        if (request.RootOnly == true)
            query = query.Where(x => x.ParentOrganizationUnitId == null);

        var count = await query.CountAsync(ct);
        var items = await query
            .OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(OrganizationUnitResponse.Projection)
            .ToListAsync(ct);

        return new PagedResult<OrganizationUnitResponse>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = count
        };
    }
}

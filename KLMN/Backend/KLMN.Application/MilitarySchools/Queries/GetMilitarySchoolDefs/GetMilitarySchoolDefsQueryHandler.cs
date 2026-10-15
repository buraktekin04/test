using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.MilitarySchools.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitarySchools.Queries.GetMilitarySchoolDefs;

/// <summary>Arama, iki FK filtresi ve sayfalama işlemlerini PostgreSQL'de uygular.</summary>
public sealed class GetMilitarySchoolDefsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitarySchoolDefsQuery, PagedResult<MilitarySchoolDefResponse>>
{
    /// <summary>Soft-delete kayıtlarını dışlayarak okul listesini projekte eder.</summary>
    public async Task<PagedResult<MilitarySchoolDefResponse>> Handle(GetMilitarySchoolDefsQuery request, CancellationToken ct)
    {
        var query = db.MilitarySchoolDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (request.IncludeInactive || request.IsActive == false || x.IsActive));
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.Code.ToUpper().Contains(search) ||
                x.Name.ToUpper().Contains(search) ||
                (x.City != null && x.City.Name.ToUpper().Contains(search)) ||
                (x.ForcesCommand != null && x.ForcesCommand.Name.ToUpper().Contains(search)));
        }
        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive);
        if (request.CityId.HasValue)
            query = query.Where(x => x.CityId == request.CityId);
        if (request.ForcesCommandDefId.HasValue)
            query = query.Where(x => x.ForcesCommandDefId == request.ForcesCommandDefId);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(MilitarySchoolDefResponse.Projection).ToListAsync(ct);
        return new PagedResult<MilitarySchoolDefResponse>
        { Items = items, TotalCount = total, PageNumber = request.PageNumber, PageSize = request.PageSize };
    }
}

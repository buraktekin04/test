using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.MilitaryClasses.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryClasses.Queries.GetMilitaryClassDefs;

/// <summary>Arama, filtreleme ve sayfalamayı sunucu tarafında PostgreSQL'e uygular.</summary>
public sealed class GetMilitaryClassDefsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryClassDefsQuery, PagedResult<MilitaryClassDefResponse>>
{
    /// <summary>Silinmemiş askerî sınıfları BaseEntity.IsActive filtresiyle listeler.</summary>
    public async Task<PagedResult<MilitaryClassDefResponse>> Handle(GetMilitaryClassDefsQuery request, CancellationToken ct)
    {
        var query = db.MilitaryClassDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted &&
                (request.IncludeInactive || request.IsActive == false || x.IsActive));
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.Code.ToUpper().Contains(search) ||
                x.Name.ToUpper().Contains(search) ||
                (x.ShortName != null && x.ShortName.ToUpper().Contains(search)) ||
                (x.ExternalCode != null && x.ExternalCode.ToUpper().Contains(search)));
        }
        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(MilitaryClassDefResponse.Projection).ToListAsync(ct);
        return new PagedResult<MilitaryClassDefResponse>
        { Items = items, TotalCount = total, PageNumber = request.PageNumber, PageSize = request.PageSize };
    }
}

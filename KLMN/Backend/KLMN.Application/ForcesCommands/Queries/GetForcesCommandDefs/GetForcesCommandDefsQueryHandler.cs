using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.ForcesCommands.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.ForcesCommands.Queries.GetForcesCommandDefs;

/// <summary>Aktif/pasif dahil filtreleri tüm veritabanına uygular ve sayfalar.</summary>
public sealed class GetForcesCommandDefsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetForcesCommandDefsQuery, PagedResult<ForcesCommandDefResponse>>
{
    public async Task<PagedResult<ForcesCommandDefResponse>> Handle(
        GetForcesCommandDefsQuery request, CancellationToken ct)
    {
        var query = request.IncludeInactive || request.IsActive == false
            ? db.ForcesCommandDefs.IgnoreQueryFilters().Where(x => !x.IsDeleted)
            : db.ForcesCommandDefs.IgnoreQueryFilters().Where(x => !x.IsDeleted && x.IsActive);

        query = query.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.Code.ToUpper().Contains(search) || x.Name.ToUpper().Contains(search));
        }
        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        var count = await query.CountAsync(ct);
        var items = await GetForcesCommandDefsSorting.Definition.Apply(query, request.SortField, request.SortOrder)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(ForcesCommandDefResponse.Projection).ToListAsync(ct);
        return new PagedResult<ForcesCommandDefResponse>
        {
            Items = items, PageNumber = request.PageNumber,
            PageSize = request.PageSize, TotalCount = count
        };
    }
}

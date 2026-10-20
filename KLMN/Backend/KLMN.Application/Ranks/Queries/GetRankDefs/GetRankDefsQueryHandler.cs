using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.Ranks.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Ranks.Queries.GetRankDefs;

/// <summary>Diğer tanım listeleriyle aynı arama ve sunucu sayfalamasını kullanır.</summary>
public sealed class GetRankDefsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetRankDefsQuery, PagedResult<RankDefResponse>>
{
    /// <summary>Silinmiş rütbeları hariç tutar; Code/Name aramasını SQL'e uygular.</summary>
    public async Task<PagedResult<RankDefResponse>> Handle(GetRankDefsQuery request, CancellationToken ct)
    {
        var query = db.RankDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted &&
                (request.IncludeInactive || request.IsActive == false || x.IsActive));
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.Code.ToUpper().Contains(search) ||
                x.Name.ToUpper().Contains(search) ||
                (x.ShortName != null && x.ShortName.ToUpper().Contains(search)) ||
                (x.RankGroup != null && x.RankGroup.Name.ToUpper().Contains(search)));
        }
        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(RankDefResponse.Projection).ToListAsync(ct);
        return new PagedResult<RankDefResponse>
        { Items = items, TotalCount = total, PageNumber = request.PageNumber, PageSize = request.PageSize };
    }
}

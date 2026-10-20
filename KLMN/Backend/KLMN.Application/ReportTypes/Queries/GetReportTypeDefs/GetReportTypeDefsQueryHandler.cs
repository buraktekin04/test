using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.ReportTypes.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.ReportTypes.Queries.GetReportTypeDefs;

/// <summary>Diğer tanım listeleriyle aynı arama ve sunucu sayfalamasını kullanır.</summary>
public sealed class GetReportTypeDefsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetReportTypeDefsQuery, PagedResult<ReportTypeDefResponse>>
{
    /// <summary>Silinmiş rapor türüları hariç tutar; Code/Name aramasını SQL'e uygular.</summary>
    public async Task<PagedResult<ReportTypeDefResponse>> Handle(GetReportTypeDefsQuery request, CancellationToken ct)
    {
        var query = db.ReportTypeDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted &&
                (request.IncludeInactive || request.IsActive == false || x.IsActive));
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.Code.ToUpper().Contains(search) ||
                x.Name.ToUpper().Contains(search));
        }
        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(ReportTypeDefResponse.Projection).ToListAsync(ct);
        return new PagedResult<ReportTypeDefResponse>
        { Items = items, TotalCount = total, PageNumber = request.PageNumber, PageSize = request.PageSize };
    }
}

using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.Procedures.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Procedures.Queries.GetProcedureDefs;

/// <summary>Diğer tanım listeleriyle aynı arama ve sunucu sayfalamasını kullanır.</summary>
public sealed class GetProcedureDefsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetProcedureDefsQuery, PagedResult<ProcedureDefResponse>>
{
    /// <summary>Silinmiş tetkikları hariç tutar; Code/Name aramasını SQL'e uygular.</summary>
    public async Task<PagedResult<ProcedureDefResponse>> Handle(GetProcedureDefsQuery request, CancellationToken ct)
    {
        var query = db.ProcedureDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted &&
                (request.IncludeInactive || request.IsActive == false || x.IsActive));
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.ProcedureCode.ToUpper().Contains(search) ||
                x.Name.ToUpper().Contains(search));
        }
        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Name).ThenBy(x => x.ProcedureCode)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(ProcedureDefResponse.Projection).ToListAsync(ct);
        return new PagedResult<ProcedureDefResponse>
        { Items = items, TotalCount = total, PageNumber = request.PageNumber, PageSize = request.PageSize };
    }
}

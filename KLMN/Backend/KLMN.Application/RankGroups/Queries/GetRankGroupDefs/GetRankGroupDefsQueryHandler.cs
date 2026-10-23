using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.RankGroups.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.RankGroups.Queries.GetRankGroupDefs;

/// <summary>Diğer tanım listeleriyle aynı arama ve sunucu sayfalamasını kullanır.</summary>
public sealed class GetRankGroupDefsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetRankGroupDefsQuery, PagedResult<RankGroupDefResponse>>
{
    /// <summary>Silinmiş rütbe grubuları hariç tutar; Code/Name aramasını SQL'e uygular.</summary>
    public async Task<PagedResult<RankGroupDefResponse>> Handle(GetRankGroupDefsQuery request, CancellationToken ct)
    {
        var query = db.RankGroupDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted &&
                (request.IncludeInactive || request.IsActive == false || x.IsActive));
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.Code.ToUpper().Contains(search) ||
                x.Name.ToUpper().Contains(search) ||
                (x.ShortName != null && x.ShortName.ToUpper().Contains(search)));
        }
        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(x =>
        new RankGroupDefResponse
        {
            Id = x.Id, Code = x.Code, Name = x.Name, ShortName = x.ShortName,
            ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, IsActive = x.IsActive,
            CreatedDate = x.CreatedDate, CreatedBy = x.CreatedBy,
            UpdatedDate = x.UpdatedDate, UpdatedBy = x.UpdatedBy, Version = x.Version
        }).ToListAsync(ct);
        return new PagedResult<RankGroupDefResponse>
        { Items = items, TotalCount = total, PageNumber = request.PageNumber, PageSize = request.PageSize };
    }
}

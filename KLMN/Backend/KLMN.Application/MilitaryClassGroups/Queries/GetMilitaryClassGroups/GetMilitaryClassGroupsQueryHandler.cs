using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.MilitaryClassGroups.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryClassGroups.Queries.GetMilitaryClassGroups;

/// <summary>Grup filtrelerini PostgreSQL tarafında işler; kolon sıralaması ön yüzde kalır.</summary>
public sealed class GetMilitaryClassGroupsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryClassGroupsQuery, PagedResult<MilitaryClassGroupResponse>>
{
    /// <summary>Soft-delete olmayan grupları kuvvet adına göre de aranabilir biçimde döner.</summary>
    public async Task<PagedResult<MilitaryClassGroupResponse>> Handle(GetMilitaryClassGroupsQuery request, CancellationToken ct)
    {
        var query = db.MilitaryClassGroupDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (request.IncludeInactive || request.IsActive == false || x.IsActive));
        if (!string.IsNullOrWhiteSpace(request.Search)) {
            var term = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.Code.ToUpper().Contains(term) || x.Name.ToUpper().Contains(term)
                || (x.ShortName != null && x.ShortName.ToUpper().Contains(term))
                || x.ForcesCommand.Name.ToUpper().Contains(term));
        }
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive);
        if (request.ForcesCommandId.HasValue) query = query.Where(x => x.ForcesCommandId == request.ForcesCommandId);
        var count = await query.CountAsync(ct);
        var data = await query.OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(x =>
        new MilitaryClassGroupResponse {
            Id = x.Id, Code = x.Code, Name = x.Name, ShortName = x.ShortName,
            ForcesCommandId = x.ForcesCommandId, ForcesCommandName = x.ForcesCommand.Name,
            IsActive = x.IsActive, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo,
            Version = x.Version, CreatedDate = x.CreatedDate
        }).ToListAsync(ct);
        return new PagedResult<MilitaryClassGroupResponse>
        { Items = data, TotalCount = count, PageNumber = request.PageNumber, PageSize = request.PageSize };
    }
}

using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.MilitaryClassGroupRelations.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryClassGroupRelations.Queries.GetMilitaryClassGroupRelations;

/// <summary>İlişkileri PostgreSQL'de filtreleyip sayfalar; PrimeNG sıralaması istemcide kalır.</summary>
public sealed class GetMilitaryClassGroupRelationsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryClassGroupRelationsQuery, PagedResult<MilitaryClassGroupRelationResponse>>
{
    /// <summary>Kuvvet, sınıf ve grup adında arama yapar.</summary>
    public async Task<PagedResult<MilitaryClassGroupRelationResponse>> Handle(GetMilitaryClassGroupRelationsQuery request, CancellationToken ct)
    {
        var query = db.MilitaryClassGroupRelations.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (request.IncludeInactive || request.IsActive == false || x.IsActive));
        if (!string.IsNullOrWhiteSpace(request.Search)) {
            var term = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.ForcesCommand.Name.ToUpper().Contains(term)
                || x.MilitaryClass.Name.ToUpper().Contains(term) || x.MilitaryClassGroup.Name.ToUpper().Contains(term));
        }
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive);
        if (request.ForcesCommandId.HasValue) query = query.Where(x => x.ForcesCommandId == request.ForcesCommandId);
        if (request.MilitaryClassId.HasValue) query = query.Where(x => x.MilitaryClassId == request.MilitaryClassId);
        if (request.MilitaryClassGroupId.HasValue) query = query.Where(x => x.MilitaryClassGroupId == request.MilitaryClassGroupId);
        var count = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.ForcesCommand.Name).ThenBy(x => x.MilitaryClass.Name)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(x =>
        new MilitaryClassGroupRelationResponse {
            Id = x.Id, ForcesCommandId = x.ForcesCommandId, ForcesCommandName = x.ForcesCommand.Name,
            MilitaryClassId = x.MilitaryClassId, MilitaryClassName = x.MilitaryClass.Name,
            MilitaryClassGroupId = x.MilitaryClassGroupId, MilitaryClassGroupName = x.MilitaryClassGroup.Name,
            IsActive = x.IsActive, Version = x.Version, CreatedDate = x.CreatedDate
        }).ToListAsync(ct);
        return new PagedResult<MilitaryClassGroupRelationResponse>
        { Items = items, TotalCount = count, PageNumber = request.PageNumber, PageSize = request.PageSize };
    }
}

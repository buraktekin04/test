using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClassGroups.Exceptions;
using KLMN.Application.MilitaryClassGroups.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryClassGroups.Queries.GetMilitaryClassGroupById;

/// <summary>Grup detayını, soft-delete filtreli EF Core projection ile bulur.</summary>
public sealed class GetMilitaryClassGroupByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryClassGroupByIdQuery, MilitaryClassGroupResponse>
{
    /// <summary>Bulunamayan grup için 404 tetikler.</summary>
    public async Task<MilitaryClassGroupResponse> Handle(GetMilitaryClassGroupByIdQuery query, CancellationToken ct) =>
        await db.MilitaryClassGroupDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == query.Id && !x.IsDeleted).Select(MilitaryClassGroupResponse.Projection)
            .SingleOrDefaultAsync(ct) ?? throw new MilitaryClassGroupNotFoundException();
}

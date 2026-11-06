using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Provisions.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Provisions.Queries.GetProvisionStatuses;

/// <summary>Durum adları ve sırası tanım tablosundan gelir.</summary>
public sealed class GetProvisionStatusesQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetProvisionStatusesQuery,IReadOnlyList<ProvisionStatusOption>>
{
    public async Task<IReadOnlyList<ProvisionStatusOption>> Handle(
        GetProvisionStatusesQuery request,CancellationToken ct)
    {
        return await db.ProvisionStatusDefs.AsNoTracking()
            .OrderBy(x=>x.SortOrder)
            .ThenBy(x=>x.Code)
            .Select(x=>new ProvisionStatusOption(x.Code,x.Name,x.SortOrder))
            .ToListAsync(ct);
    }
}

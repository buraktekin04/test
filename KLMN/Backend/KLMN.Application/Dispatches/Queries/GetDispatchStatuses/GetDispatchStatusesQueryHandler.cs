using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Dispatches.Queries.GetDispatchStatuses;

/// <summary>Durum adlarını UI'ye sabitlemeden DB sözlük tablosundan okur.</summary>
public sealed class GetDispatchStatusesQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetDispatchStatusesQuery,IReadOnlyList<DispatchStatusOptionResponse>>
{
    public async Task<IReadOnlyList<DispatchStatusOptionResponse>> Handle(
        GetDispatchStatusesQuery request,CancellationToken ct)
    {
        return await db.DispatchStatusDefs.AsNoTracking()
            .Where(x=>x.IsActive)
            .OrderBy(x=>x.SortOrder)
            .ThenBy(x=>x.Code)
            .Select(x=>new DispatchStatusOptionResponse(x.Code,x.Name))
            .ToListAsync(ct);
    }
}

using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Dispatches.Queries.GetDispatchGenders;

/// <summary>Cinsiyet seçeneklerini sadece etkin ve silinmemiş DB kayıtlarından okur.</summary>
public sealed class GetDispatchGendersQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetDispatchGendersQuery,IReadOnlyList<DispatchGenderOptionResponse>>
{
    public async Task<IReadOnlyList<DispatchGenderOptionResponse>> Handle(
        GetDispatchGendersQuery request,CancellationToken ct)
    {
        return await db.GenderDefs.AsNoTracking()
            .Where(x=>!x.IsDeleted&&x.IsActive)
            .OrderBy(x=>x.SortOrder).ThenBy(x=>x.Code)
            .Select(x=>new DispatchGenderOptionResponse(x.Code,x.Name))
            .ToListAsync(ct);
    }
}

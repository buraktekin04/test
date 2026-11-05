using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Provisions.Exceptions;
using KLMN.Application.Provisions.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Provisions.Queries.GetProvisionStates;

/// <summary>Yerel provizyon durum tarihçesi; bakanlık servis sonuçları ayrıca entegre edilecek.</summary>
public sealed class GetProvisionStatesQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetProvisionStatesQuery,IReadOnlyList<ProvisionStateResponse>>
{
    public async Task<IReadOnlyList<ProvisionStateResponse>> Handle(GetProvisionStatesQuery request,CancellationToken ct)
    {
        if(!await db.Provisions.AnyAsync(x=>x.Id==request.Id&&!x.IsDeleted,ct))
            throw new ProvisionNotFoundException();
        return await db.ProvisionStates.AsNoTracking()
            .Where(x=>x.ProvisionId==request.Id&&!x.IsDeleted)
            .OrderBy(x=>x.CreatedDate)
            .Select(x=>new ProvisionStateResponse(x.Id,x.State,x.CreatedDate,x.CreatedBy))
            .ToListAsync(ct);
    }
}

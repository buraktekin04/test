using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryUnits.Exceptions;
using KLMN.Application.MilitaryUnits.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitDefById;

/// <summary>Pasifler dahil soft-delete edilmemiş birimi getirir.</summary>
public sealed class GetMilitaryUnitDefByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryUnitDefByIdQuery, MilitaryUnitDefResponse>
{
    public async Task<MilitaryUnitDefResponse> Handle(GetMilitaryUnitDefByIdQuery request, CancellationToken ct) =>
        await db.MilitaryUnitDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(MilitaryUnitDefResponse.Projection)
            .SingleOrDefaultAsync(ct) ?? throw new MilitaryUnitNotFoundException();
}

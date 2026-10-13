using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Organizations.Exceptions;
using KLMN.Application.Organizations.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Organizations.Queries.GetOrganizationUnitById;

/// <summary>Pasifler dahil soft-delete edilmemiş birimi getirir.</summary>
public sealed class GetOrganizationUnitByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetOrganizationUnitByIdQuery, OrganizationUnitResponse>
{
    public async Task<OrganizationUnitResponse> Handle(GetOrganizationUnitByIdQuery request, CancellationToken ct) =>
        await db.OrganizationUnits.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(OrganizationUnitResponse.Projection)
            .SingleOrDefaultAsync(ct) ?? throw new OrganizationNotFoundException();
}

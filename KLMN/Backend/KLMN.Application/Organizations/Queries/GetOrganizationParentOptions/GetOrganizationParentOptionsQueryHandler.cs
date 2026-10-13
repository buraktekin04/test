using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Organizations.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Organizations.Queries.GetOrganizationParentOptions;

/// <summary>Organizasyon seçicisinde yalnızca aktif birimleri gösterir.</summary>
public sealed class GetOrganizationParentOptionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetOrganizationParentOptionsQuery, IReadOnlyList<OrganizationParentOptionResponse>>
{
    public async Task<IReadOnlyList<OrganizationParentOptionResponse>> Handle(
        GetOrganizationParentOptionsQuery request, CancellationToken ct) =>
        await db.OrganizationUnits.AsNoTracking()
            .OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Select(x => new OrganizationParentOptionResponse(
                x.Id, x.Code, x.Name, x.ParentOrganizationUnitId))
            .ToListAsync(ct);
}

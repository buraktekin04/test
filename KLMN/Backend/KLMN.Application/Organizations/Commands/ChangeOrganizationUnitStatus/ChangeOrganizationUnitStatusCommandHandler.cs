using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Organizations.Commands.Common;
using KLMN.Application.Organizations.Exceptions;
using KLMN.Application.Organizations.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Organizations.Commands.ChangeOrganizationUnitStatus;

/// <summary>Bağlı aktif kullanıcı veya alt birim varsa pasifleştirmeyi engeller.</summary>
public sealed class ChangeOrganizationUnitStatusCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeOrganizationUnitStatusCommand, OrganizationUnitMutationResponse>
{
    public async Task<OrganizationUnitMutationResponse> Handle(ChangeOrganizationUnitStatusCommand request, CancellationToken ct)
    {
        var unit = await OrganizationUnitRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (!request.IsActive)
        {
            var activeUsers = await db.Users.IgnoreQueryFilters()
                .AnyAsync(x => x.OrganizationUnitId == unit.Id && !x.IsDeleted && x.IsActive, ct);
            var activeChildren = await db.OrganizationUnits.IgnoreQueryFilters()
                .AnyAsync(x => x.ParentOrganizationUnitId == unit.Id && !x.IsDeleted && x.IsActive, ct);
            if (activeUsers || activeChildren)
                throw new OrganizationConflictException(
                    "Aktif kullanıcı veya aktif alt birim bulunan organizasyon pasifleştirilemez.");
        }
        else if (unit.ParentOrganizationUnitId.HasValue)
        {
            await OrganizationUnitRules.EnsureValidParentAsync(db, unit.ParentOrganizationUnitId, unit.Id, ct);
        }

        unit.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new OrganizationUnitMutationResponse(unit.Id, unit.Version);
    }
}

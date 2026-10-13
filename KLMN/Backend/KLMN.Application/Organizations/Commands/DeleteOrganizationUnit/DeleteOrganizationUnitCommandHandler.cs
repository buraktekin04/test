using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Organizations.Commands.Common;
using KLMN.Application.Organizations.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Organizations.Commands.DeleteOrganizationUnit;

/// <summary>İlişkili kullanıcı/alt birim varsa silmeyen, güvenli soft-delete komutudur.</summary>
public sealed class DeleteOrganizationUnitCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteOrganizationUnitCommand>
{
    public async Task Handle(DeleteOrganizationUnitCommand request, CancellationToken ct)
    {
        var unit = await OrganizationUnitRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        // Pasif kullanıcılar ve pasif alt birimler de FK ilişkisini sürdürür.
        var hasUsers = await db.Users.IgnoreQueryFilters()
            .AnyAsync(x => x.OrganizationUnitId == unit.Id && !x.IsDeleted, ct);
        var hasChildren = await db.OrganizationUnits.IgnoreQueryFilters()
            .AnyAsync(x => x.ParentOrganizationUnitId == unit.Id && !x.IsDeleted, ct);
        if (hasUsers || hasChildren)
            throw new OrganizationConflictException(
                "Bağlı kullanıcıları veya alt birimleri bulunan organizasyon silinemez.");

        db.OrganizationUnits.Remove(unit);
        await db.SaveChangesAsync(ct);
    }
}

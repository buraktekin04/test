using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryUnits.Commands.Common;
using KLMN.Application.MilitaryUnits.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryUnits.Commands.DeleteMilitaryUnitDef;

/// <summary>İlişkili kullanıcı/alt birim varsa silmeyen, güvenli soft-delete komutudur.</summary>
public sealed class DeleteMilitaryUnitDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteMilitaryUnitDefCommand>
{
    public async Task Handle(DeleteMilitaryUnitDefCommand request, CancellationToken ct)
    {
        var unit = await MilitaryUnitDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        // Pasif kullanıcılar ve pasif alt birimler de FK ilişkisini sürdürür.
        var hasUsers = await db.Users.IgnoreQueryFilters()
            .AnyAsync(x => x.MilitaryUnitDefId == unit.Id && !x.IsDeleted, ct);
        var hasChildren = await db.MilitaryUnitDefs.IgnoreQueryFilters()
            .AnyAsync(x => x.ConnectedTopUnity == unit.Id && !x.IsDeleted, ct);
        if (hasUsers || hasChildren)
            throw new MilitaryUnitConflictException(
                "Bağlı kullanıcıları veya alt birimleri bulunan birlik silinemez.");

        db.MilitaryUnitDefs.Remove(unit);
        await db.SaveChangesAsync(ct);
    }
}

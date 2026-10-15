using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClasses.Commands.Common;

using MediatR;
using Microsoft.EntityFrameworkCore;
using KLMN.Application.MilitaryClasses.Exceptions;

namespace KLMN.Application.MilitaryClasses.Commands.DeleteMilitaryClassDef;

/// <summary>Sınıfı AuditableEntitySaveChangesInterceptor ile soft-delete yapar.</summary>
public sealed class DeleteMilitaryClassDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteMilitaryClassDefCommand>
{
    /// <summary>Güncel xmin'i doğrular ve entity değişikliğini kaydeder.</summary>
    public async Task Handle(DeleteMilitaryClassDefCommand request, CancellationToken ct)
    {
        var item = await MilitaryClassDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (await db.MilitaryClassGroupRelations.IgnoreQueryFilters().AnyAsync(x =>
            x.MilitaryClassId == item.Id && !x.IsDeleted, ct))
            throw new MilitaryClassConflictException("Eşleştirmesi bulunan askerî sınıf silinemez.");
        db.MilitaryClassDefs.Remove(item);
        await db.SaveChangesAsync(ct);

    }
}

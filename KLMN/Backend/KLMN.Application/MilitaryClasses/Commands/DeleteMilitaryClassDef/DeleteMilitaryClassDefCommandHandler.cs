using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClasses.Commands.Common;

using MediatR;

namespace KLMN.Application.MilitaryClasses.Commands.DeleteMilitaryClassDef;

/// <summary>Sınıfı AuditableEntitySaveChangesInterceptor ile soft-delete yapar.</summary>
public sealed class DeleteMilitaryClassDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteMilitaryClassDefCommand>
{
    /// <summary>Güncel xmin'i doğrular ve entity değişikliğini kaydeder.</summary>
    public async Task Handle(DeleteMilitaryClassDefCommand request, CancellationToken ct)
    {
        var item = await MilitaryClassDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        db.MilitaryClassDefs.Remove(item);
        await db.SaveChangesAsync(ct);

    }
}

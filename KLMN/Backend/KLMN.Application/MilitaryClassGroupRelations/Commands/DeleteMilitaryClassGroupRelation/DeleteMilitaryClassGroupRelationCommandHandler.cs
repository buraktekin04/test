using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClassGroupRelations.Commands.Common;

using MediatR;

namespace KLMN.Application.MilitaryClassGroupRelations.Commands.DeleteMilitaryClassGroupRelation;

/// <summary>Tek ilişkiyi audit ve soft-delete davranışıyla kaldırır.</summary>
public sealed class DeleteMilitaryClassGroupRelationCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteMilitaryClassGroupRelationCommand>
{
    /// <summary>xmin kontrolü ile eşleştirmeyi değiştirir.</summary>
    public async Task Handle(DeleteMilitaryClassGroupRelationCommand request, CancellationToken ct)
    {
        var item = await MilitaryClassGroupRelationRules.LoadAsync(db, request.Id, request.Version, ct);
        db.MilitaryClassGroupRelations.Remove(item);
        await db.SaveChangesAsync(ct);

    }
}

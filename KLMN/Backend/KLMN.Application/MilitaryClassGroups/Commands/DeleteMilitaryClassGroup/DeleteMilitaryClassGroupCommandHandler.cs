using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClassGroups.Commands.Common;

using MediatR;

namespace KLMN.Application.MilitaryClassGroups.Commands.DeleteMilitaryClassGroup;

/// <summary>Grup silinmeden önce bağlı eşleştirme olmadığını denetler.</summary>
public sealed class DeleteMilitaryClassGroupCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteMilitaryClassGroupCommand>
{
    /// <summary>İlişki kısıtları ve xmin kontrolü sonrası kayıt güncellenir.</summary>
    public async Task Handle(DeleteMilitaryClassGroupCommand request, CancellationToken ct)
    {
        var item = await MilitaryClassGroupRules.LoadAsync(db, request.Id, request.Version, ct);
        await MilitaryClassGroupRules.EnsureNoRelationsAsync(db, item.Id, false, ct);
        db.MilitaryClassGroupDefs.Remove(item);
        await db.SaveChangesAsync(ct);

    }
}

using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClassGroups.Commands.Common;
using KLMN.Application.MilitaryClassGroups.Responses;
using MediatR;

namespace KLMN.Application.MilitaryClassGroups.Commands.ChangeStatusMilitaryClassGroup;

/// <summary>Bağlı eşleştirmelerin aktifliğini koruyarak grup durumunu değiştirir.</summary>
public sealed class ChangeStatusMilitaryClassGroupCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusMilitaryClassGroupCommand, MilitaryClassGroupMutationResponse>
{
    /// <summary>İlişki kısıtları ve xmin kontrolü sonrası kayıt güncellenir.</summary>
    public async Task<MilitaryClassGroupMutationResponse> Handle(ChangeStatusMilitaryClassGroupCommand request, CancellationToken ct)
    {
        var item = await MilitaryClassGroupRules.LoadAsync(db, request.Id, request.Version, ct);
        if (request.IsActive)
            await MilitaryClassGroupRules.EnsureForceAsync(db, item.ForcesCommandId, ct);
        else
            await MilitaryClassGroupRules.EnsureNoRelationsAsync(db, item.Id, true, ct);
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new MilitaryClassGroupMutationResponse(item.Id, item.Version);
    }
}

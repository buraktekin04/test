using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClassGroups.Commands.Common;
using KLMN.Application.MilitaryClassGroups.Responses;

using MediatR;

namespace KLMN.Application.MilitaryClassGroups.Commands.UpdateMilitaryClassGroup;

/// <summary>Grup ve kuvvet bağlantısını referans bütünlüğünü bozmadan günceller.</summary>
public sealed class UpdateMilitaryClassGroupCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdateMilitaryClassGroupCommand, MilitaryClassGroupMutationResponse>
{
    /// <summary>Kod/kuvvet/xmin kurallarını denetledikten sonra kaydı saklar.</summary>
    public async Task<MilitaryClassGroupMutationResponse> Handle(UpdateMilitaryClassGroupCommand request, CancellationToken ct)
    {
        var item = await MilitaryClassGroupRules.LoadAsync(db, request.Id, request.Version, ct);
        if (item.ForcesCommandId != request.ForcesCommandId)
            await MilitaryClassGroupRules.EnsureNoRelationsAsync(db, item.Id, false, ct);
        await MilitaryClassGroupRules.EnsureForceAsync(db, request.ForcesCommandId, ct);
        await MilitaryClassGroupRules.EnsureUniqueAsync(db, request.ForcesCommandId, request.Code, item.Id, ct);
        item.ForcesCommandId = request.ForcesCommandId;
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.ShortName = string.IsNullOrWhiteSpace(request.ShortName) ? null : request.ShortName.Trim();
        item.ValidFrom = request.ValidFrom;
        item.ValidTo = request.ValidTo;

        await db.SaveChangesAsync(ct);
        return new MilitaryClassGroupMutationResponse(item.Id, item.Version);
    }
}

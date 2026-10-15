using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClassGroupRelations.Commands.Common;
using KLMN.Application.MilitaryClassGroupRelations.Responses;

using MediatR;

namespace KLMN.Application.MilitaryClassGroupRelations.Commands.UpdateMilitaryClassGroupRelation;

/// <summary>Eşleştirmeyi değiştirmeden önce üç FK ve mevcut xmin'i denetler.</summary>
public sealed class UpdateMilitaryClassGroupRelationCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdateMilitaryClassGroupRelationCommand, MilitaryClassGroupRelationMutationResponse>
{
    /// <summary>İş kurallarını denetler ve audit interceptor aracılığıyla kaydeder.</summary>
    public async Task<MilitaryClassGroupRelationMutationResponse> Handle(UpdateMilitaryClassGroupRelationCommand request, CancellationToken ct)
    {
        var item = await MilitaryClassGroupRelationRules.LoadAsync(db, request.Id, request.Version, ct);
        await MilitaryClassGroupRelationRules.EnsureReferencesAsync(
            db, request.ForcesCommandId, request.MilitaryClassId, request.MilitaryClassGroupId, ct);
        await MilitaryClassGroupRelationRules.EnsureUniqueAsync(
            db, request.ForcesCommandId, request.MilitaryClassId, request.MilitaryClassGroupId, item.Id, ct);
        item.ForcesCommandId = request.ForcesCommandId;
        item.MilitaryClassId = request.MilitaryClassId;
        item.MilitaryClassGroupId = request.MilitaryClassGroupId;

        await db.SaveChangesAsync(ct);
        return new MilitaryClassGroupRelationMutationResponse(item.Id, item.Version);
    }
}

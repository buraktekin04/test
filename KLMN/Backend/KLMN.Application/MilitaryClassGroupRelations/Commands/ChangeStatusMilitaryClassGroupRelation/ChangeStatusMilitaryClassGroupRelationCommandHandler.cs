using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClassGroupRelations.Commands.Common;
using KLMN.Application.MilitaryClassGroupRelations.Responses;
using MediatR;

namespace KLMN.Application.MilitaryClassGroupRelations.Commands.ChangeStatusMilitaryClassGroupRelation;

/// <summary>Aktif eşleştirmenin kuvvet/grup/sınıfının aktif kalmasını denetler.</summary>
public sealed class ChangeStatusMilitaryClassGroupRelationCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusMilitaryClassGroupRelationCommand, MilitaryClassGroupRelationMutationResponse>
{
    /// <summary>xmin kontrolü ile eşleştirmeyi değiştirir.</summary>
    public async Task<MilitaryClassGroupRelationMutationResponse> Handle(ChangeStatusMilitaryClassGroupRelationCommand request, CancellationToken ct)
    {
        var item = await MilitaryClassGroupRelationRules.LoadAsync(db, request.Id, request.Version, ct);
        if (request.IsActive) await MilitaryClassGroupRelationRules.EnsureReferencesAsync(
            db, item.ForcesCommandId, item.MilitaryClassId, item.MilitaryClassGroupId, ct);
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new MilitaryClassGroupRelationMutationResponse(item.Id, item.Version);
    }
}

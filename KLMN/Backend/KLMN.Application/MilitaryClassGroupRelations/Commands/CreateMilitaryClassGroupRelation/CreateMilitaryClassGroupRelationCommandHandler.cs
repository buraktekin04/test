using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClassGroupRelations.Commands.Common;
using KLMN.Application.MilitaryClassGroupRelations.Responses;
using KLMN.Domain.Entities.MilitaryUnit;
using MediatR;

namespace KLMN.Application.MilitaryClassGroupRelations.Commands.CreateMilitaryClassGroupRelation;

/// <summary>Çift kaydı ve farklı kuvvete ait grup seçimini reddederek ilişki ekler.</summary>
public sealed class CreateMilitaryClassGroupRelationCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateMilitaryClassGroupRelationCommand, MilitaryClassGroupRelationMutationResponse>
{
    /// <summary>İş kurallarını denetler ve audit interceptor aracılığıyla kaydeder.</summary>
    public async Task<MilitaryClassGroupRelationMutationResponse> Handle(CreateMilitaryClassGroupRelationCommand request, CancellationToken ct)
    {
        var item = new MilitaryClassGroupRelation();
        await MilitaryClassGroupRelationRules.EnsureReferencesAsync(
            db, request.ForcesCommandId, request.MilitaryClassId, request.MilitaryClassGroupId, ct);
        await MilitaryClassGroupRelationRules.EnsureUniqueAsync(
            db, request.ForcesCommandId, request.MilitaryClassId, request.MilitaryClassGroupId, null, ct);
        item.ForcesCommandId = request.ForcesCommandId;
        item.MilitaryClassId = request.MilitaryClassId;
        item.MilitaryClassGroupId = request.MilitaryClassGroupId;
        await db.MilitaryClassGroupRelations.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return new MilitaryClassGroupRelationMutationResponse(item.Id, item.Version);
    }
}

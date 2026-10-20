using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Ranks.Commands.Common;
using KLMN.Application.Ranks.Responses;
using MediatR;

namespace KLMN.Application.Ranks.Commands.ChangeStatusRankDef;

/// <summary>Rütbe ChangeStatus iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class ChangeStatusRankDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusRankDefCommand, RankDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<RankDefMutationResponse> Handle(ChangeStatusRankDefCommand request, CancellationToken ct)
    {
        var item = await RankDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (request.IsActive)
            await RankDefRules.EnsureGroupAsync(db, item.RankGroupId, ct);
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new RankDefMutationResponse(item.Id, item.Version);
    }
}

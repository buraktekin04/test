using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.RankGroups.Commands.Common;
using KLMN.Application.RankGroups.Responses;
using MediatR;

namespace KLMN.Application.RankGroups.Commands.ChangeStatusRankGroupDef;

/// <summary>Rütbe Grubu ChangeStatus iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class ChangeStatusRankGroupDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusRankGroupDefCommand, RankGroupDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<RankGroupDefMutationResponse> Handle(ChangeStatusRankGroupDefCommand request, CancellationToken ct)
    {
        var item = await RankGroupDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (!request.IsActive)
            await RankGroupDefRules.EnsureNoRanksAsync(db, item.Id, deactivate: true, ct);
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new RankGroupDefMutationResponse(item.Id, item.Version);
    }
}

using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.RankGroups.Commands.Common;
using MediatR;

namespace KLMN.Application.RankGroups.Commands.DeleteRankGroupDef;

/// <summary>Rütbe Grubu Delete iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class DeleteRankGroupDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteRankGroupDefCommand>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task Handle(DeleteRankGroupDefCommand request, CancellationToken ct)
    {
        var item = await RankGroupDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await RankGroupDefRules.EnsureNoRanksAsync(db, item.Id, deactivate: false, ct);
        db.RankGroupDefs.Remove(item);
        await db.SaveChangesAsync(ct);
    }
}

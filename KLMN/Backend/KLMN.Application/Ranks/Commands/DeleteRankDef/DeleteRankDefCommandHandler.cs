using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Ranks.Commands.Common;
using MediatR;

namespace KLMN.Application.Ranks.Commands.DeleteRankDef;

/// <summary>Rütbe Delete iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class DeleteRankDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteRankDefCommand>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task Handle(DeleteRankDefCommand request, CancellationToken ct)
    {
        var item = await RankDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        db.RankDefs.Remove(item);
        await db.SaveChangesAsync(ct);
    }
}

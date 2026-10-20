using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.RankGroups.Commands.Common;
using KLMN.Application.RankGroups.Responses;
using MediatR;

namespace KLMN.Application.RankGroups.Commands.UpdateRankGroupDef;

/// <summary>Rütbe Grubu Update iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class UpdateRankGroupDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdateRankGroupDefCommand, RankGroupDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<RankGroupDefMutationResponse> Handle(UpdateRankGroupDefCommand request, CancellationToken ct)
    {
        var item = await RankGroupDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await RankGroupDefRules.EnsureUniqueCodeAsync(db, request.Code, item.Id, ct);
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.ShortName = string.IsNullOrWhiteSpace(request.ShortName) ? null : request.ShortName.Trim();
        item.ValidFrom = request.ValidFrom;
        item.ValidTo = request.ValidTo;
        await db.SaveChangesAsync(ct);
        return new RankGroupDefMutationResponse(item.Id, item.Version);
    }
}

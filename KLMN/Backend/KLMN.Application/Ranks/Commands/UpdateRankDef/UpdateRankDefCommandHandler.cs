using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Ranks.Commands.Common;
using KLMN.Application.Ranks.Responses;
using MediatR;

namespace KLMN.Application.Ranks.Commands.UpdateRankDef;

/// <summary>Rütbe Update iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class UpdateRankDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdateRankDefCommand, RankDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<RankDefMutationResponse> Handle(UpdateRankDefCommand request, CancellationToken ct)
    {
        var item = await RankDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await RankDefRules.EnsureUniqueCodeAsync(db, request.Code, item.Id, ct);
        await RankDefRules.EnsureGroupAsync(db, request.RankGroupId, ct, item.RankGroupId);
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.ShortName = string.IsNullOrWhiteSpace(request.ShortName) ? null : request.ShortName.Trim();
        item.Type = request.Type;
        item.RankGroupId = request.RankGroupId;
        item.ValidFrom = request.ValidFrom;
        item.ValidTo = request.ValidTo;
        await db.SaveChangesAsync(ct);
        return new RankDefMutationResponse(item.Id, item.Version);
    }
}

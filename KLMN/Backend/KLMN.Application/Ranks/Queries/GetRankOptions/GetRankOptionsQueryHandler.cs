using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Ranks.Queries.GetRankOptions;

/// <summary>Aktif ve silinmemiş gruplar; düzenlemede seçili pasif grup da gösterilir.</summary>
public sealed class GetRankOptionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetRankOptionsQuery, IReadOnlyList<RankOptionResponse>>
{
    /// <summary>Yalnız seçime gerekli alanları getirir.</summary>
    public async Task<IReadOnlyList<RankOptionResponse>> Handle(GetRankOptionsQuery request, CancellationToken ct) =>
        await db.RankGroupDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (x.IsActive || x.Id == request.SelectedRankGroupId))
            .OrderBy(x => x.Name)
            .Select(x => new RankOptionResponse(x.Id, x.Code, x.Name)).ToListAsync(ct);
}
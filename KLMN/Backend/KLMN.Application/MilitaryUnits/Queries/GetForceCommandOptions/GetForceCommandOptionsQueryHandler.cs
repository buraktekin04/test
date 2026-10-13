using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryUnits.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryUnits.Queries.GetForceCommandOptions;

/// <summary>Yalnızca aktif ve geçerli dönemdeki kuvvetleri birlik seçimine sunar.</summary>
public sealed class GetForceCommandOptionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetForceCommandOptionsQuery, IReadOnlyList<ForceCommandOptionResponse>>
{
    public async Task<IReadOnlyList<ForceCommandOptionResponse>> Handle(GetForceCommandOptionsQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        return await db.ForcesCommandDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsActive &&
                (!x.ValidFrom.HasValue || x.ValidFrom <= now) && (!x.ValidTo.HasValue || now < x.ValidTo))
            .OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Select(x => new ForceCommandOptionResponse(x.Id, x.Code, x.Name)).ToListAsync(ct);
    }
}

using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryUnits.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryUnits.Queries.GetForceCommandOptions;

/// <summary>Aynı sorguyla form için aktif/geçerli, filtre için silinmemiş kuvvetleri sunar.</summary>
public sealed class GetForceCommandOptionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetForceCommandOptionsQuery, IReadOnlyList<ForceCommandOptionResponse>>
{
    public async Task<IReadOnlyList<ForceCommandOptionResponse>> Handle(GetForceCommandOptionsQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var query = db.ForcesCommandDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted);
        if (!request.IncludeInactive)
            query = query.Where(x => x.IsActive &&
                (!x.ValidFrom.HasValue || x.ValidFrom <= now) &&
                (!x.ValidTo.HasValue || now < x.ValidTo));

        return await query.OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Select(x => new ForceCommandOptionResponse(x.Id, x.Code, x.Name)).ToListAsync(ct);
    }
}

using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryClassGroups.Queries.GetMilitaryClassGroupOptions;

/// <summary>Yalnızca grup modülü izniyle erişilen kuvvet dropdown'udur.</summary>
public sealed class GetMilitaryClassGroupOptionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryClassGroupOptionsQuery, IReadOnlyList<MilitaryClassGroupForceOption>>
{
    /// <summary>Aktif kuvvetleri ve düzenlemede mevcut pasif seçimi gösterir.</summary>
    public async Task<IReadOnlyList<MilitaryClassGroupForceOption>> Handle(GetMilitaryClassGroupOptionsQuery request, CancellationToken ct) =>
        await db.ForcesCommandDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (x.IsActive || x.Id == request.SelectedForceId))
            .OrderBy(x => x.Name).Select(x => new MilitaryClassGroupForceOption(x.Id, x.Code, x.Name))
            .ToListAsync(ct);
}

using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryClassGroupRelations.Queries.GetMilitaryClassGroupRelationOptions;

/// <summary>İlişki modülü izniyle kuvvet, sınıf ve seçilen kuvvete ait grup dropdown'unu sunar.</summary>
public sealed class GetMilitaryClassGroupRelationOptionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryClassGroupRelationOptionsQuery, MilitaryClassGroupRelationOptions>
{
    /// <summary>Düzenlemede seçili eski pasif kayıtlar görünür, yeni bağlantılar aktiflerle sınırlandırılır.</summary>
    public async Task<MilitaryClassGroupRelationOptions> Handle(GetMilitaryClassGroupRelationOptionsQuery q, CancellationToken ct)
    {
        var forces = await db.ForcesCommandDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (x.IsActive || x.Id == q.SelectedForceId))
            .OrderBy(x => x.Name)
            .Select(x => new MilitaryClassGroupRelationOption(x.Id, x.Code, x.Name)).ToListAsync(ct);
        var classes = await db.MilitaryClassDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (x.IsActive || x.Id == q.SelectedClassId))
            .OrderBy(x => x.Name)
            .Select(x => new MilitaryClassGroupRelationOption(x.Id, x.Code, x.Name)).ToListAsync(ct);
        var groups = await db.MilitaryClassGroupDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (x.ForcesCommandId == q.ForcesCommandId || x.Id == q.SelectedGroupId)
                && (x.IsActive || x.Id == q.SelectedGroupId))
            .OrderBy(x => x.Name)
            .Select(x => new MilitaryClassGroupRelationOption(x.Id, x.Code, x.Name)).ToListAsync(ct);
        return new MilitaryClassGroupRelationOptions(forces, classes, groups);
    }
}

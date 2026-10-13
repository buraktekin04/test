using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Users.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Users.Queries.GetMilitaryUnitDefOptions;

/// <summary>
/// Kullanıcı formu için yalnızca aktif ve silinmemiş birlik birimlerini
/// seçilebilir seçenek olarak döndüren CQRS sorgu işleyicisidir.
/// </summary>
public sealed class GetMilitaryUnitDefOptionsQueryHandler(
    IKLMNDbContext dbContext)
    : IRequestHandler<GetMilitaryUnitDefOptionsQuery, IReadOnlyList<MilitaryUnitDefOptionResponse>>
{
    /// <summary>
    /// Görev yeri seçeneklerini kod ve isim bazında sıralar. Kullanıcıya
    /// atanacak birim bilgileri dışında kurum içi hassas veri aktarılmaz.
    /// </summary>
    public async Task<IReadOnlyList<MilitaryUnitDefOptionResponse>> Handle(
        GetMilitaryUnitDefOptionsQuery request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        return await dbContext.MilitaryUnitDefs
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.IsActive && !x.IsDeleted &&
                (!x.ValidFrom.HasValue || x.ValidFrom <= now) &&
                (!x.ValidTo.HasValue || now < x.ValidTo))
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Code)
            .Select(x => new MilitaryUnitDefOptionResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                ConnectedTopUnity = x.ConnectedTopUnity
            })
            .ToListAsync(cancellationToken);
    }
}

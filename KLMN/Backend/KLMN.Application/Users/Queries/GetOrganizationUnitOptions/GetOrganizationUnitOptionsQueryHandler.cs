using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Users.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Users.Queries.GetOrganizationUnitOptions;

/// <summary>
/// Kullanıcı formu için yalnızca aktif ve silinmemiş organizasyon birimlerini
/// seçilebilir seçenek olarak döndüren CQRS sorgu işleyicisidir.
/// </summary>
public sealed class GetOrganizationUnitOptionsQueryHandler(
    IKLMNDbContext dbContext)
    : IRequestHandler<GetOrganizationUnitOptionsQuery, IReadOnlyList<OrganizationUnitOptionResponse>>
{
    /// <summary>
    /// Görev yeri seçeneklerini kod ve isim bazında sıralar. Kullanıcıya
    /// atanacak birim bilgileri dışında kurum içi hassas veri aktarılmaz.
    /// </summary>
    public async Task<IReadOnlyList<OrganizationUnitOptionResponse>> Handle(
        GetOrganizationUnitOptionsQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.OrganizationUnits
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.IsActive && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Code)
            .Select(x => new OrganizationUnitOptionResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                ParentOrganizationUnitId = x.ParentOrganizationUnitId
            })
            .ToListAsync(cancellationToken);
    }
}

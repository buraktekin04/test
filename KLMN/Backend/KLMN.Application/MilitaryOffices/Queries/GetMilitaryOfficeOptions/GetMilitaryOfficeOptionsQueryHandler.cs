using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryOffices.Queries.GetMilitaryOfficeOptions;

/// <summary>Aktif şehirler ve düzenlemede seçili pasif şehir için seçenekler.</summary>
public sealed class GetMilitaryOfficeOptionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryOfficeOptionsQuery, IReadOnlyList<MilitaryOfficeOptionResponse>>
{
    /// <summary>Şube ekranının kendi yetkisiyle sınırlı şehir verisini getirir.</summary>
    public async Task<IReadOnlyList<MilitaryOfficeOptionResponse>> Handle(
        GetMilitaryOfficeOptionsQuery request, CancellationToken ct)
    {
        return await db.CityDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (x.IsActive || x.Id == request.SelectedCityId))
            .OrderBy(x => x.Name)
            .Select(x => new MilitaryOfficeOptionResponse(x.Id, x.Code, x.Name))
            .ToListAsync(ct);
    }
}

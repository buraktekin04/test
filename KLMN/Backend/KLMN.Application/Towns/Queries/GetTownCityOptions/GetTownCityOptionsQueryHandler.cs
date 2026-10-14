using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Towns.Queries.GetTownCityOptions;
/// <summary>
/// İlçe formu için aktif şehirleri listeler; düzenlenen eski şehir pasif olsa
/// bile mevcut seçimin kaybolmaması için isteğe bağlı olarak onu dahil eder.
/// </summary>
public sealed class GetTownCityOptionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetTownCityOptionsQuery, IReadOnlyList<TownCityOptionResponse>>
{
    public async Task<IReadOnlyList<TownCityOptionResponse>> Handle(GetTownCityOptionsQuery request, CancellationToken ct) =>
        await db.CityDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (x.IsActive || x.Id == request.SelectedCityId))
            .OrderBy(x => x.Name).Select(x => new TownCityOptionResponse(x.Id, x.Code, x.Name)).ToListAsync(ct);
}

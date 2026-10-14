using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Cities.Queries.GetCityCountryOptions;
public sealed class GetCityCountryOptionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetCityCountryOptionsQuery, IReadOnlyList<CityCountryOptionResponse>>
{
    public async Task<IReadOnlyList<CityCountryOptionResponse>> Handle(GetCityCountryOptionsQuery request, CancellationToken ct) =>
        await db.CountryDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (x.IsActive || x.Id == request.SelectedCountryId))
            .OrderBy(x => x.Name).Select(x => new CityCountryOptionResponse(x.Id, x.Code, x.Name)).ToListAsync(ct);
}

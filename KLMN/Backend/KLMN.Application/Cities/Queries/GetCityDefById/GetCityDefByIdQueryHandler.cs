using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Cities.Exceptions;
using KLMN.Application.Cities.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Cities.Queries.GetCityDefById;
public sealed class GetCityDefByIdQueryHandler(IKLMNDbContext db) : IRequestHandler<GetCityDefByIdQuery, CityDefResponse>
{
    public async Task<CityDefResponse> Handle(GetCityDefByIdQuery request, CancellationToken ct) =>
        await db.CityDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(CityDefResponse.Projection).SingleOrDefaultAsync(ct)
            ?? throw new CityNotFoundException();
}

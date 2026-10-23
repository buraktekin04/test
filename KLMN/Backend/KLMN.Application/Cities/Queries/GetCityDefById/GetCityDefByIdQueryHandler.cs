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
            .Select(x =>
        new CityDefResponse
        {
            Id=x.Id, Code=x.Code, Name=x.Name, ExternalCode=x.ExternalCode,
            CountryId=x.CountryId, CountryName=x.Country == null ? null : x.Country.Name,
            MernisCode=x.MernisCode, ValidFrom=x.ValidFrom, ValidTo=x.ValidTo,
            IsActive=x.IsActive, CreatedDate=x.CreatedDate, CreatedBy=x.CreatedBy,
            UpdatedDate=x.UpdatedDate, UpdatedBy=x.UpdatedBy, Version=x.Version
        }).SingleOrDefaultAsync(ct)
            ?? throw new CityNotFoundException();
}

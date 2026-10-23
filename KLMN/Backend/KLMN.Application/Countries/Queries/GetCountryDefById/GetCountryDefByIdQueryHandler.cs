using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Countries.Exceptions;
using KLMN.Application.Countries.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Countries.Queries.GetCountryDefById;
public sealed class GetCountryDefByIdQueryHandler(IKLMNDbContext db) : IRequestHandler<GetCountryDefByIdQuery, CountryDefResponse>
{
    public async Task<CountryDefResponse> Handle(GetCountryDefByIdQuery request, CancellationToken ct) =>
        await db.CountryDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(x =>
        new CountryDefResponse
        {
            Id=x.Id, Code=x.Code, Name=x.Name, ExternalCode=x.ExternalCode, ShortCode=x.ShortCode,
            IsTownRequired=x.IsTownRequired, CityCount=x.Cities.Count(c => !c.IsDeleted),
            IsActive=x.IsActive, CreatedDate=x.CreatedDate, CreatedBy=x.CreatedBy,
            UpdatedDate=x.UpdatedDate, UpdatedBy=x.UpdatedBy, Version=x.Version
        }).SingleOrDefaultAsync(ct)
            ?? throw new CountryNotFoundException();
}

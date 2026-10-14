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
            .Select(CountryDefResponse.Projection).SingleOrDefaultAsync(ct)
            ?? throw new CountryNotFoundException();
}

using KLMN.Application.Common.Models;
using KLMN.Application.Cities.Responses;
using MediatR;
namespace KLMN.Application.Cities.Queries.GetCityDefs;
public sealed record GetCityDefsQuery(
    string? Search = null, bool IncludeInactive = false, int PageNumber = 1,
    int PageSize = 20, bool? IsActive = null, Guid? CountryId = null)
    : IRequest<PagedResult<CityDefResponse>>;

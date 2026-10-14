using KLMN.Application.Common.Models;
using KLMN.Application.Countries.Responses;
using MediatR;
namespace KLMN.Application.Countries.Queries.GetCountryDefs;
public sealed record GetCountryDefsQuery(
    string? Search = null, bool IncludeInactive = false, int PageNumber = 1,
    int PageSize = 20, bool? IsActive = null, bool? IsTownRequired = null)
    : IRequest<PagedResult<CountryDefResponse>>;

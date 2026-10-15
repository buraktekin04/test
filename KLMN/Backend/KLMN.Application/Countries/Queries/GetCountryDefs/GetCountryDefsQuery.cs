using KLMN.Application.Common.Models;
using KLMN.Application.Countries.Responses;
using MediatR;
namespace KLMN.Application.Countries.Queries.GetCountryDefs;
/// <summary>Ülke sorgusunda ShortCode sütun filtresi ve mevcut filtrelerin tamamı.</summary>
public sealed record GetCountryDefsQuery(
    string? Search = null, bool IncludeInactive = false, int PageNumber = 1,
    int PageSize = 20, bool? IsActive = null, bool? IsTownRequired = null, string? ShortCode = null)
    : IRequest<PagedResult<CountryDefResponse>>;

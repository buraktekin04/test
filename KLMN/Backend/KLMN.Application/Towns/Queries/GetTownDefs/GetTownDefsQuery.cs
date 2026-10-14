using KLMN.Application.Common.Models;
using KLMN.Application.Towns.Responses;
using MediatR;
namespace KLMN.Application.Towns.Queries.GetTownDefs;
/// <summary>İlçe tanımına ait sorgu, yanıt veya iş kuralı bileşeni.</summary>
public sealed record GetTownDefsQuery(
    string? Search = null, bool IncludeInactive = false, int PageNumber = 1,
    int PageSize = 20, bool? IsActive = null, Guid? CityId = null)
    : IRequest<PagedResult<TownDefResponse>>;

using KLMN.Application.Common.Models;
using KLMN.Application.Ranks.Responses;
using MediatR;

namespace KLMN.Application.Ranks.Queries.GetRankDefs;

/// <summary>Arama, durum filtresi ve sayfa bilgileri.</summary>
public sealed record GetRankDefsQuery(
    string? Search = null, bool IncludeInactive = false,
    int PageNumber = 1, int PageSize = 20, bool? IsActive = null)
    : IRequest<PagedResult<RankDefResponse>>;

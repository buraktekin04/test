using KLMN.Application.Common.Models;
using KLMN.Application.RankGroups.Responses;
using MediatR;

namespace KLMN.Application.RankGroups.Queries.GetRankGroupDefs;

/// <summary>Arama, durum filtresi ve sayfa bilgileri.</summary>
public sealed record GetRankGroupDefsQuery(
    string? Search = null, bool IncludeInactive = false,
    int PageNumber = 1, int PageSize = 20, bool? IsActive = null)
    : IRequest<PagedResult<RankGroupDefResponse>>;

using KLMN.Application.Common.Models;
using KLMN.Application.Branches.Responses;
using MediatR;

namespace KLMN.Application.Branches.Queries.GetBranchDefs;

/// <summary>Arama, durum filtresi ve sayfa bilgileri.</summary>
public sealed record GetBranchDefsQuery(
    string? Search = null, bool IncludeInactive = false,
    int PageNumber = 1, int PageSize = 20, bool? IsActive = null)
    : IRequest<PagedResult<BranchDefResponse>>;

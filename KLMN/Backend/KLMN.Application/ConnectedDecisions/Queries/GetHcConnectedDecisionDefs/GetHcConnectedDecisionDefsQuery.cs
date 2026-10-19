using KLMN.Application.Common.Models;
using KLMN.Application.ConnectedDecisions.Responses;
using MediatR;

namespace KLMN.Application.ConnectedDecisions.Queries.GetHcConnectedDecisionDefs;

/// <summary>Arama, durum filtresi ve sayfa bilgileri.</summary>
public sealed record GetHcConnectedDecisionDefsQuery(
    string? Search = null, bool IncludeInactive = false,
    int PageNumber = 1, int PageSize = 20, bool? IsActive = null)
    : IRequest<PagedResult<HcConnectedDecisionDefResponse>>;

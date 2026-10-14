using KLMN.Application.Common.Models;
using KLMN.Application.PurchaseNumbers.Responses;
using MediatR;

namespace KLMN.Application.PurchaseNumbers.Queries.GetPurchaseNumberDefs;

/// <summary>Filtreler PostgreSQL üzerinde çalışır; sıralama tabloda istemciye aittir.</summary>
public sealed record GetPurchaseNumberDefsQuery(
    string? Search = null, bool IncludeInactive = false,
    int PageNumber = 1, int PageSize = 20,
    bool? IsActive = null, string? SupplyDescription = null)
    : IRequest<PagedResult<PurchaseNumberDefResponse>>;

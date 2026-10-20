using KLMN.Application.Common.Models;
using KLMN.Application.Procedures.Responses;
using MediatR;

namespace KLMN.Application.Procedures.Queries.GetProcedureDefs;

/// <summary>Arama, durum filtresi ve sayfa bilgileri.</summary>
public sealed record GetProcedureDefsQuery(
    string? Search = null, bool IncludeInactive = false,
    int PageNumber = 1, int PageSize = 20, bool? IsActive = null)
    : IRequest<PagedResult<ProcedureDefResponse>>;

using KLMN.Application.Common.Models;
using KLMN.Application.Hospitals.Responses;
using MediatR;

namespace KLMN.Application.Hospitals.Queries.GetHospitalDefs;

/// <summary>Arama, durum filtresi ve sayfa bilgileri.</summary>
public sealed record GetHospitalDefsQuery(
    string? Search = null, bool IncludeInactive = false,
    int PageNumber = 1, int PageSize = 20, bool? IsActive = null)
    : IRequest<PagedResult<HospitalDefResponse>>;

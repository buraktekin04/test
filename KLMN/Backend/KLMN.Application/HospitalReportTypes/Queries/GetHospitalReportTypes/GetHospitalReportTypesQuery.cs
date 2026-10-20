using KLMN.Application.Common.Models;
using KLMN.Application.HospitalReportTypes.Responses;
using MediatR;

namespace KLMN.Application.HospitalReportTypes.Queries.GetHospitalReportTypes;

/// <summary>Arama, durum filtresi ve sayfa bilgileri.</summary>
public sealed record GetHospitalReportTypesQuery(
    string? Search = null, bool IncludeInactive = false,
    int PageNumber = 1, int PageSize = 20, bool? IsActive = null)
    : IRequest<PagedResult<HospitalReportTypeResponse>>;

using KLMN.Application.Common.Models;
using KLMN.Application.HospitalReportTypes.Responses;
using MediatR;

namespace KLMN.Application.HospitalReportTypes.Queries.GetHospitalReportTypeGroups;

/// <summary>Sayfa başına eşleştirme değil, sağlık tesisi sayısını esas alan arama.</summary>
public sealed record GetHospitalReportTypeGroupsQuery(
    string? Search = null,
    bool IncludeInactive = false,
    int PageNumber = 1,
    int PageSize = 20,
    bool? IsActive = null)
    : IRequest<PagedResult<HospitalReportTypeGroupResponse>>;

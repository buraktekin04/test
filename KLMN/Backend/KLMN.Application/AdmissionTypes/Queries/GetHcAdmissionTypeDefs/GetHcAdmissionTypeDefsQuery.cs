using KLMN.Application.Common.Models;
using KLMN.Application.AdmissionTypes.Responses;
using MediatR;

namespace KLMN.Application.AdmissionTypes.Queries.GetHcAdmissionTypeDefs;

/// <summary>Başvuru gruplarını arama, durum ve sayfa bazında listeler.</summary>
public sealed record GetHcAdmissionTypeDefsQuery(
    string? Search = null, bool IncludeInactive = false,
    int PageNumber = 1, int PageSize = 20, bool? IsActive = null)
    : IRequest<PagedResult<HcAdmissionTypeDefResponse>>;

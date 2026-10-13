using KLMN.Application.Common.Models;
using KLMN.Application.Organizations.Responses;
using MediatR;

namespace KLMN.Application.Organizations.Queries.GetOrganizationUnits;

/// <summary>Organizasyon tanımlarında arama, filtreleme ve sayfalama sorgusudur.</summary>
public sealed record GetOrganizationUnitsQuery(
    string? Search = null,
    bool IncludeInactive = false,
    int PageNumber = 1,
    int PageSize = 20,
    bool? IsActive = null,
    bool? RootOnly = null)
    : IRequest<PagedResult<OrganizationUnitResponse>>;

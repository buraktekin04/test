using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.Roles.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Roles.Queries.GetRoles;

/// <summary>Veritabanındaki tüm roller üzerinde filtreleme ve sunucu sayfalaması uygular.</summary>
public sealed record GetRolesQuery(
    string? Search = null,
    bool IncludeInactive = false,
    int PageNumber = 1,
    int PageSize = 20,
    bool? IsActive = null,
    bool? IsSystemRole = null,
    string SortField = "name",
    int SortOrder = 1)
    : IRequest<PagedResult<RoleResponse>>;

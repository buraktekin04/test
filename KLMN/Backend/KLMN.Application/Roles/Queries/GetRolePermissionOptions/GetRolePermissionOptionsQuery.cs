using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Roles.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Roles.Queries.GetRolePermissionOptions;

/// <summary>Yetki seçiciye aktif permission kataloğunu getirir.</summary>
public sealed record GetRolePermissionOptionsQuery()
    : IRequest<IReadOnlyList<RolePermissionOptionResponse>>;

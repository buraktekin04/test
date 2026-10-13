using KLMN.Application.Roles.Responses;
using MediatR;

namespace KLMN.Application.Roles.Commands.UpdateRole;

/// <summary>Rol temel alanlarını xmin concurrency denetimiyle günceller.</summary>
public sealed record UpdateRoleCommand(
    Guid Id, uint Version, string Name, string Code, string? Description)
    : IRequest<RoleMutationResponse>;

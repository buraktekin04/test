using KLMN.Application.Roles.Responses;
using MediatR;

namespace KLMN.Application.Roles.Commands;

/// <summary>Yeni rol adı, kodu ve açıklamasını sunucuya iletir.</summary>
public sealed record CreateRoleCommand(
    string Name, string Code, string? Description) : IRequest<RoleMutationResponse>;

/// <summary>Rol temel alanlarını xmin concurrency denetimiyle günceller.</summary>
public sealed record UpdateRoleCommand(
    Guid Id, uint Version, string Name, string Code, string? Description)
    : IRequest<RoleMutationResponse>;

/// <summary>Sistem dışı rolün aktifleştirilmesi veya pasifleştirilmesi isteğidir.</summary>
public sealed record ChangeRoleStatusCommand(Guid Id, uint Version, bool IsActive)
    : IRequest<RoleMutationResponse>;

/// <summary>Rol ilişkileri korunarak uygulanan soft-delete isteğidir.</summary>
public sealed record DeleteRoleCommand(Guid Id, uint Version) : IRequest;

/// <summary>Rolün permission listesini tek işlemle değiştiren, xmin kontrollü istektir.</summary>
public sealed record UpdateRolePermissionsCommand(
    Guid Id, uint Version, IReadOnlyList<Guid> PermissionIds)
    : IRequest<RoleMutationResponse>;

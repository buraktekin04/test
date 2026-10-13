using KLMN.Application.Roles.Responses;
using MediatR;

namespace KLMN.Application.Roles.Commands.UpdateRolePermissions;

/// <summary>Rolün permission listesini tek işlemle değiştiren, xmin kontrollü istektir.</summary>
public sealed record UpdateRolePermissionsCommand(
    Guid Id, uint Version, IReadOnlyList<Guid> PermissionIds)
    : IRequest<RoleMutationResponse>;

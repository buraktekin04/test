using KLMN.Application.Roles.Responses;
using MediatR;

namespace KLMN.Application.Roles.Commands.ChangeRoleStatus;

/// <summary>Sistem dışı rolün aktifleştirilmesi veya pasifleştirilmesi isteğidir.</summary>
public sealed record ChangeRoleStatusCommand(Guid Id, uint Version, bool IsActive)
    : IRequest<RoleMutationResponse>;

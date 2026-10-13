using KLMN.Application.Roles.Responses;
using MediatR;

namespace KLMN.Application.Roles.Commands.CreateRole;

/// <summary>Yeni rol adı, kodu ve açıklamasını sunucuya iletir.</summary>
public sealed record CreateRoleCommand(
    string Name, string Code, string? Description) : IRequest<RoleMutationResponse>;

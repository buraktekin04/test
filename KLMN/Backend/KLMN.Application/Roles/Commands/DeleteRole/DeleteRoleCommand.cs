using KLMN.Application.Roles.Responses;
using MediatR;

namespace KLMN.Application.Roles.Commands.DeleteRole;

/// <summary>Rol ilişkileri korunarak uygulanan soft-delete isteğidir.</summary>
public sealed record DeleteRoleCommand(Guid Id, uint Version) : IRequest;

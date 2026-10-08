using MediatR;

namespace KLMN.Application.Authentication.Commands.ChangePassword;

/// <summary>Authenticated kullanıcının parolasını değiştiren command modelidir.</summary>
public sealed record ChangePasswordCommand : IRequest
{
    public required string CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
    public required string ConfirmPassword { get; init; }
}

using MediatR;

namespace KLMN.Application.Authentication.Commands.ResetPassword;

/// <summary>Reset token kullanarak yeni parola belirleme command modelidir.</summary>
public sealed record ResetPasswordCommand : IRequest
{
    public required string Token { get; init; }
    public required string NewPassword { get; init; }
    public required string ConfirmPassword { get; init; }
}

using MediatR;

namespace KLMN.Application.Authentication.Commands.ResetPassword;

/// <summary>
/// Parola sıfırlama token'ı ile yeni parola belirler.
/// </summary>
public sealed record ResetPasswordCommand(
    string Token,
    string NewPassword,
    string ConfirmPassword)
    : IRequest;

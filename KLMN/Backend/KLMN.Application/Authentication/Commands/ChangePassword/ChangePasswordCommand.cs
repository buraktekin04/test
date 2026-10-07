using MediatR;

namespace KLMN.Application.Authentication.Commands.ChangePassword;

/// <summary>
/// Authenticated kullanıcının kendi parolasını değiştirir.
/// </summary>
public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword)
    : IRequest;

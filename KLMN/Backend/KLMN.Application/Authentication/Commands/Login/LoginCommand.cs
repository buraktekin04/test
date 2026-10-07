using KLMN.Application.Authentication.Models;
using MediatR;

namespace KLMN.Application.Authentication.Commands.Login;

/// <summary>
/// Kullanıcı adı/e-posta ve parola ile login işlemini başlatır.
/// </summary>
public sealed record LoginCommand(
    string UserNameOrEmail,
    string Password,
    string? IpAddress = null,
    string? UserAgent = null,
    string? DeviceName = null)
    : IRequest<AuthSessionResult>;

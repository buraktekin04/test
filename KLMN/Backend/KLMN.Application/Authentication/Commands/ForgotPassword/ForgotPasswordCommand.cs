using MediatR;

namespace KLMN.Application.Authentication.Commands.ForgotPassword;

/// <summary>
/// E-posta adresi için parola sıfırlama bağlantısı talep eder.
/// </summary>
public sealed record ForgotPasswordCommand(
    string Email)
    : IRequest;

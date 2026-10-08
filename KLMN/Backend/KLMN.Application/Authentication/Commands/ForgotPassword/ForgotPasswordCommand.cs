using MediatR;

namespace KLMN.Application.Authentication.Commands.ForgotPassword;

/// <summary>E-posta adresine parola sıfırlama bağlantısı gönderme command modelidir.</summary>
public sealed record ForgotPasswordCommand : IRequest
{
    public required string Email { get; init; }
}

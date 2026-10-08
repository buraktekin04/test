using MediatR;

namespace KLMN.Application.Authentication.Commands.ForgotPassword;

/// <summary>E-posta adresine parola sıfırlama bağlantısı gönderme command modelidir.</summary>
public sealed record ForgotPasswordCommand : IRequest
{
    /// <summary>
    /// İlgili kullanıcı hesabına ulaşmak için kullanılan e-posta adresidir.
    /// </summary>
    public required string Email { get; init; }
}

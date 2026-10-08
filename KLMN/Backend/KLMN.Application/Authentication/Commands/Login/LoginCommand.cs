using KLMN.Application.Authentication.Models;
using MediatR;

namespace KLMN.Application.Authentication.Commands.Login;

/// <summary>
/// Kullanıcı adı veya e-posta ve parola kullanılarak sisteme
/// giriş yapılmasını sağlayan CQRS command modelidir.
/// </summary>
public sealed record LoginCommand
    : IRequest<AuthSessionResult>
{
    /// <summary>
    /// Kullanıcının kullanıcı adı veya e-posta adresidir.
    /// </summary>
    public required string Identifier { get; init; }

    /// <summary>
    /// Kullanıcının açık giriş parolasıdır.
    /// Yalnızca doğrulama amacıyla kullanılır ve saklanmaz.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// Kullanıcının giriş yaptığı cihazın okunabilir adıdır.
    /// Örneğin "Web", "Chrome / Windows" veya mobil cihaz adı olabilir.
    /// </summary>
    public string? DeviceName { get; init; }
}

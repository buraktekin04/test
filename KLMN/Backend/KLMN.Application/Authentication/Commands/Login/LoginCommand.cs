using KLMN.Application.Authentication.Responses;
using MediatR;

namespace KLMN.Application.Authentication.Commands.Login;

/// <summary>Kullanıcı adı/e-posta ve parola ile giriş command modelidir.</summary>
public sealed record LoginCommand : IRequest<LoginResult>
{
    /// <summary>
    /// Giriş sırasında kullanıcı adı veya e-posta olarak girilen tanımlayıcıdır.
    /// </summary>
    public required string Identifier { get; init; }
    /// <summary>
    /// Yalnızca doğrulama amacıyla kullanılan açık paroladır; kalıcı saklanmaz.
    /// </summary>
    public required string Password { get; init; }
    /// <summary>
    /// Oturumu oluşturan tarayıcı veya cihazın kullanıcıya gösterilebilir adıdır.
    /// </summary>
    public string? DeviceName { get; init; }
}

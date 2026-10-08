using KLMN.Application.Authentication.Responses;
using MediatR;

namespace KLMN.Application.Authentication.Commands.Login;

/// <summary>Kullanıcı adı/e-posta ve parola ile giriş command modelidir.</summary>
public sealed record LoginCommand : IRequest<LoginResult>
{
    public required string Identifier { get; init; }
    public required string Password { get; init; }
    public string? DeviceName { get; init; }
}

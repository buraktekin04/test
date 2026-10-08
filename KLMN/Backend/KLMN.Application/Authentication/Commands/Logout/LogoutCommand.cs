using MediatR;

namespace KLMN.Application.Authentication.Commands.Logout;

/// <summary>Mevcut refresh token oturumunu sonlandırır.</summary>
public sealed record LogoutCommand : IRequest
{
    /// <summary>
    /// İstemcinin HttpOnly cookie içinde taşıdığı açık refresh token değeridir.
    /// </summary>
    public required string RefreshToken { get; init; }
}

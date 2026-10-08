using MediatR;

namespace KLMN.Application.Authentication.Commands.Logout;

/// <summary>Mevcut refresh token oturumunu sonlandırır.</summary>
public sealed record LogoutCommand : IRequest
{
    public required string RefreshToken { get; init; }
}

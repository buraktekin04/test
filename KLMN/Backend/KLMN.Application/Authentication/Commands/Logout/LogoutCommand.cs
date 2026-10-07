using MediatR;

namespace KLMN.Application.Authentication.Commands.Logout;

/// <summary>
/// Mevcut cihazdaki refresh token'ı revoke eder.
/// </summary>
public sealed record LogoutCommand(
    string? RefreshToken,
    string? IpAddress = null)
    : IRequest;

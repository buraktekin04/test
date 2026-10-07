using KLMN.Application.Authentication.Models;
using MediatR;

namespace KLMN.Application.Authentication.Commands.RefreshToken;

/// <summary>
/// HttpOnly cookie içerisindeki refresh token ile session yeniler.
/// </summary>
public sealed record RefreshTokenCommand(
    string RefreshToken,
    string? IpAddress = null,
    string? UserAgent = null,
    string? DeviceName = null)
    : IRequest<AuthSessionResult>;

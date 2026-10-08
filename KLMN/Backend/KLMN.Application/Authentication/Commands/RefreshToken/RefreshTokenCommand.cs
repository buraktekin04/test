using KLMN.Application.Authentication.Responses;
using MediatR;

namespace KLMN.Application.Authentication.Commands.RefreshToken;

/// <summary>Refresh token ile yeni access/refresh token üretme command modelidir.</summary>
public sealed record RefreshTokenCommand : IRequest<RefreshResult>
{
    public required string RefreshToken { get; init; }
}

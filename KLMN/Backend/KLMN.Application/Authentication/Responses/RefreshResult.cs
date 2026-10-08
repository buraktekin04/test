namespace KLMN.Application.Authentication.Responses;

/// <summary>Refresh handler tarafından API katmanına gönderilen sonucu temsil eder.</summary>
public sealed record RefreshResult
{
    public required RefreshResponse Response { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTime RefreshTokenExpiresAt { get; init; }
}

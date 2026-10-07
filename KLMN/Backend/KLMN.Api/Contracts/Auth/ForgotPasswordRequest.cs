namespace KLMN.Api.Contracts.Auth;

/// <summary>
/// Forgot password endpoint request modelidir.
/// </summary>
public sealed record ForgotPasswordRequest(
    string Email);

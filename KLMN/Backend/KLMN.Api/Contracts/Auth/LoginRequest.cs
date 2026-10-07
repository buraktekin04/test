namespace KLMN.Api.Contracts.Auth;

/// <summary>
/// Login endpoint request modelidir.
/// </summary>
public sealed record LoginRequest(
    string UserNameOrEmail,
    string Password);

namespace KLMN.Api.Contracts.Auth;

/// <summary>
/// Change password endpoint request modelidir.
/// </summary>
public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword);

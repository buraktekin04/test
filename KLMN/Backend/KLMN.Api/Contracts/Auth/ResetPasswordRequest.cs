namespace KLMN.Api.Contracts.Auth;

/// <summary>
/// Reset password endpoint request modelidir.
/// </summary>
public sealed record ResetPasswordRequest(
    string Token,
    string NewPassword,
    string ConfirmPassword);

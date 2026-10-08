using MediatR;

namespace KLMN.Application.Authentication.Commands.ResetPassword;

/// <summary>Reset token kullanarak yeni parola belirleme command modelidir.</summary>
public sealed record ResetPasswordCommand : IRequest
{
    /// <summary>
    /// İstemcinin kimlik doğrulama veya parola sıfırlama için gönderdiği açık tokenıdır.
    /// </summary>
    public required string Token { get; init; }
    /// <summary>
    /// Kullanıcının belirlemek istediği yeni paroladır.
    /// </summary>
    public required string NewPassword { get; init; }
    /// <summary>
    /// Yeni parolanın tekrar girilmiş doğrulama değeridir.
    /// </summary>
    public required string ConfirmPassword { get; init; }
}

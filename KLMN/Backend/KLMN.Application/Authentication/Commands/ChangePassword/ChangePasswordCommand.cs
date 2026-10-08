using MediatR;

namespace KLMN.Application.Authentication.Commands.ChangePassword;

/// <summary>Authenticated kullanıcının parolasını değiştiren command modelidir.</summary>
public sealed record ChangePasswordCommand : IRequest
{
    /// <summary>
    /// Parola güncellemesi öncesinde doğrulanacak mevcut paroladır.
    /// </summary>
    public required string CurrentPassword { get; init; }
    /// <summary>
    /// Hashlenerek kaydedilecek yeni paroladır.
    /// </summary>
    public required string NewPassword { get; init; }
    /// <summary>
    /// Yeni parolanın yanlış girilmesini önleyen doğrulama alanıdır.
    /// </summary>
    public required string ConfirmPassword { get; init; }
}

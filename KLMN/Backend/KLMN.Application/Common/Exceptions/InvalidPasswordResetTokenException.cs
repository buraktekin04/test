namespace KLMN.Application.Common.Exceptions;

/// <summary>Reset token'ın geçersiz veya süresi dolmuş olduğunu belirtir.</summary>
public sealed class InvalidPasswordResetTokenException : Exception
{
    /// <summary>
    /// invalid password reset token exception işlemini ilgili servis sözleşmesine göre yürütür.
    /// </summary>
    public InvalidPasswordResetTokenException()
        : base("Parola sıfırlama bağlantısı geçersiz veya süresi dolmuştur.")
    {
    }
}

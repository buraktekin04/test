namespace KLMN.Application.Common.Exceptions;

/// <summary>Reset token'ın geçersiz veya süresi dolmuş olduğunu belirtir.</summary>
public sealed class InvalidPasswordResetTokenException : Exception
{
    public InvalidPasswordResetTokenException()
        : base("Parola sıfırlama bağlantısı geçersiz veya süresi dolmuştur.")
    {
    }
}

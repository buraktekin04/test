namespace KLMN.Application.Common.Exceptions;

/// <summary>
/// Parola sıfırlama token'ının geçersiz veya süresi dolmuş olduğunu belirtir.
/// </summary>
public sealed class InvalidPasswordResetTokenException : Exception
{
    public InvalidPasswordResetTokenException()
        : base("Parola sıfırlama bağlantısı geçersiz veya süresi dolmuş.")
    {
    }
}

namespace KLMN.Application.Common.Exceptions;

/// <summary>
/// Refresh token'ın geçersiz, süresi dolmuş veya kullanılamaz olduğunu belirtir.
/// </summary>
public sealed class InvalidRefreshTokenException : Exception
{
    public InvalidRefreshTokenException()
        : base("Refresh token geçersiz veya süresi dolmuş.")
    {
    }
}

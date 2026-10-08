namespace KLMN.Application.Common.Exceptions;

/// <summary>Refresh token'ın kullanılamaz olduğunu belirtir.</summary>
public sealed class InvalidRefreshTokenException : Exception
{
    /// <summary>
    /// invalid refresh token exception işlemini ilgili servis sözleşmesine göre yürütür.
    /// </summary>
    public InvalidRefreshTokenException()
        : base("Oturum bilgisi geçersiz veya süresi dolmuştur.")
    {
    }
}

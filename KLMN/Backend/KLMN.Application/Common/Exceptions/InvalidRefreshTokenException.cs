namespace KLMN.Application.Common.Exceptions;

/// <summary>Refresh token'ın kullanılamaz olduğunu belirtir.</summary>
public sealed class InvalidRefreshTokenException : Exception
{
    public InvalidRefreshTokenException()
        : base("Oturum bilgisi geçersiz veya süresi dolmuştur.")
    {
    }
}

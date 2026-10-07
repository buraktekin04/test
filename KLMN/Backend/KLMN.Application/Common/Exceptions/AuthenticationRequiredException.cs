namespace KLMN.Application.Common.Exceptions;

/// <summary>
/// İşlem için authenticated kullanıcı gerektiğini belirtir.
/// </summary>
public sealed class AuthenticationRequiredException : Exception
{
    public AuthenticationRequiredException()
        : base("Bu işlem için oturum açmanız gerekmektedir.")
    {
    }
}

namespace KLMN.Application.Common.Exceptions;

/// <summary>İşlem için geçerli authenticated kullanıcı gerektiğini belirtir.</summary>
public sealed class AuthenticationRequiredException : Exception
{
    public AuthenticationRequiredException()
        : base("Bu işlem için geçerli bir kullanıcı oturumu gereklidir.")
    {
    }
}

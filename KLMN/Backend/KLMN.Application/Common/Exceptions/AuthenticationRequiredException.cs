namespace KLMN.Application.Common.Exceptions;

/// <summary>İşlem için geçerli authenticated kullanıcı gerektiğini belirtir.</summary>
public sealed class AuthenticationRequiredException : Exception
{
    /// <summary>
    /// authentication required exception işlemini ilgili servis sözleşmesine göre yürütür.
    /// </summary>
    public AuthenticationRequiredException()
        : base("Bu işlem için geçerli bir kullanıcı oturumu gereklidir.")
    {
    }
}

namespace KLMN.Application.Common.Exceptions;

/// <summary>Login bilgilerinin doğrulanamadığını belirtir.</summary>
public sealed class LoginFailedException : Exception
{
    /// <summary>
    /// login failed exception işlemini ilgili servis sözleşmesine göre yürütür.
    /// </summary>
    public LoginFailedException()
        : base("Kullanıcı adı/e-posta veya parola hatalıdır.")
    {
    }
}

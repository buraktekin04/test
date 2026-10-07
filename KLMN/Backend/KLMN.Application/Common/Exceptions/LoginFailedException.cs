namespace KLMN.Application.Common.Exceptions;

/// <summary>
/// Login bilgilerinin doğrulanamadığını belirtir.
/// </summary>
public sealed class LoginFailedException : Exception
{
    public LoginFailedException()
        : base("Kullanıcı adı/e-posta veya parola hatalı.")
    {
    }
}

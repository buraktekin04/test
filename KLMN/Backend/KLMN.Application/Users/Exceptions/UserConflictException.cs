namespace KLMN.Application.Users.Exceptions;

/// <summary>
/// Benzersiz kullanıcı/e-posta kuralı veya korunan hesap işlemi
/// ihlal edildiğinde 409 Conflict cevabı verilmesini sağlar.
/// </summary>
public sealed class UserConflictException : Exception
{
    /// <summary>Uygulanamayan kullanıcı işleminin güvenli açıklamasıdır.</summary>
    public UserConflictException(string message) : base(message)
    {
    }
}

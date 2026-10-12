namespace KLMN.Application.Users.Exceptions;

/// <summary>Silinmiş veya bulunamayan kullanıcı kaydını bildirir.</summary>
public sealed class UserNotFoundException : Exception
{
    /// <summary>İstemciye hassas bilgi vermeyen 404 hata mesajını oluşturur.</summary>
    public UserNotFoundException()
        : base("Kullanıcı kaydı bulunamadı.")
    {
    }
}

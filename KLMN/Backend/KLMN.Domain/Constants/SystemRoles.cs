namespace KLMN.Domain.Constants;

/// <summary>
/// Uygulamanın özel davranış uyguladığı sistem rol kodlarını içerir.
/// Dinamik olarak oluşturulan normal roller bu sınıfa eklenmez.
/// </summary>
public static class SystemRoles
{
    /// <summary>Sistem genelinde tam yetkili yönetici rol kodudur.</summary>
    public const string Admin = "ADMIN";

    /// <summary>Standart kullanıcı temel sistem rol kodudur.</summary>
    public const string StandardUser = "STANDARD_USER";
}

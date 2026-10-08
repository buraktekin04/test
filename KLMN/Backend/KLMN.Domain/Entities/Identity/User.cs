using KLMN.Domain.Common;
using KLMN.Domain.Entities.Organization;

namespace KLMN.Domain.Entities.Identity;

/// <summary>
/// Sistemde kimlik doğrulaması yapılabilen kullanıcı hesabını temsil eder.
/// </summary>
public sealed class User : BaseEntity
{
    /// <summary>Oturum açarken kullanılan kullanıcı adıdır.</summary>
    public string UserName { get; set; } = string.Empty;
    /// <summary>Büyük/küçük harf duyarsız benzersizlik ve giriş karşılaştırmasında kullanılan kullanıcı adıdır.</summary>
    public string NormalizedUserName { get; set; } = string.Empty;
    /// <summary>Açık parola saklanmaksızın güvenli algoritmayla oluşturulmuş parola hash değeridir.</summary>
    public string PasswordHash { get; set; } = string.Empty;
    /// <summary>Kullanıcının adıdır.</summary>
    public string FirstName { get; set; } = string.Empty;
    /// <summary>Kullanıcının soyadıdır.</summary>
    public string LastName { get; set; } = string.Empty;
    /// <summary>Kullanıcının zorunlu e-posta adresidir.</summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>Arama ve benzersizlik kontrollerinde kullanılan normalize edilmiş e-posta adresidir.</summary>
    public string NormalizedEmail { get; set; } = string.Empty;
    /// <summary>Kullanıcının isteğe bağlı iletişim numarasıdır.</summary>
    public string? PhoneNumber { get; set; }
    /// <summary>Kullanıcının bağlı olduğu organizasyon biriminin yabancı anahtar değeridir.</summary>
    public Guid? OrganizationUnitId { get; set; }
    /// <summary>Kullanıcının bağlı bulunduğu organizasyon birimine ilişkin navigation alanıdır.</summary>
    public OrganizationUnit? OrganizationUnit { get; set; }
    /// <summary>Ardışık başarısız giriş denemelerinin sayısıdır; kilitleme eşiğinde kullanılır.</summary>
    public int AccessFailedCount { get; set; }
    /// <summary>Kullanıcı hesabının erişime kapatılıp kapatılmadığını belirtir.</summary>
    public bool IsLocked { get; set; }
    /// <summary>Geçici hesap kilidinin sona ereceği UTC zamandır; boşsa manuel kilit olabilir.</summary>
    public DateTime? LockoutEnd { get; set; }
    /// <summary>Kullanıcı için iki faktörlü kimlik doğrulama seçeneğinin etkinliğidir.</summary>
    public bool TwoFactorAuthenticationEnabled { get; set; }
    /// <summary>Kullanıcı hesabının kullanılmaya başlanabileceği UTC zamandır.</summary>
    public DateTime? ValidFrom { get; set; }
    /// <summary>Kullanıcı hesabının geçerliliğinin biteceği UTC zamandır.</summary>
    public DateTime? ValidTo { get; set; }
    /// <summary>Kullanıcının en son başarılı giriş yaptığı UTC zamandır.</summary>
    public DateTime? LastLoginDate { get; set; }
    /// <summary>Kullanıcı parolasının en son güncellendiği UTC zamandır.</summary>
    public DateTime? PasswordChangedDate { get; set; }

    /// <summary>
    /// Parola veya kritik güvenlik bilgileri değiştiğinde yenilenen
    /// güvenlik damgasıdır.
    /// </summary>
    public string SecurityStamp { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>Kullanıcının birden fazla rolle olan bağlantılarını tutan navigation koleksiyonudur.</summary>
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    /// <summary>Kullanıcı için rol yetkisinden bağımsız tanımlanmış izin/red override kayıtlarıdır.</summary>
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    /// <summary>Kullanıcıya ait refresh token ve cihaz oturumu kayıtlarıdır.</summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    /// <summary>Kullanıcı adına üretilen tek kullanımlık parola sıfırlama token kayıtlarıdır.</summary>
    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
}

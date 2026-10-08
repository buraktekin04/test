using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Identity;

/// <summary>Tek kullanımlık parola sıfırlama token kaydını temsil eder.</summary>
public sealed class PasswordResetToken : BaseEntity
{
    /// <summary>İlgili kaydın bağlı olduğu kullanıcının benzersiz kimliğidir.</summary>
    public Guid UserId { get; set; }
    /// <summary>Açık token yerine veritabanında saklanan SHA-256 hash değeridir.</summary>
    public string TokenHash { get; set; } = string.Empty;
    /// <summary>Güvenlik tokenının kullanılamaz duruma geleceği UTC zamandır.</summary>
    public DateTime ExpiresAt { get; set; }
    /// <summary>Parola sıfırlama tokenının başarılı işlemde tüketildiği UTC zamandır.</summary>
    public DateTime? UsedAt { get; set; }
    /// <summary>Güvenlik tokenının sunucu tarafından iptal edildiği UTC zamandır.</summary>
    public DateTime? RevokedAt { get; set; }
    /// <summary>Bu ilişkinin veya güvenlik tokenının ait olduğu kullanıcı navigation alanıdır.</summary>
    public User User { get; set; } = null!;

    /// <summary>Geçerli UTC zamanının token bitiş tarihini geçip geçmediğini hesaplar; veritabanına yazılmaz.</summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    /// <summary>Parola sıfırlama tokenının daha önce kullanılıp kullanılmadığını hesaplar.</summary>
    public bool IsUsed => UsedAt.HasValue;
    /// <summary>Tokenın iptal edilip edilmediğini hesaplar; veritabanına yazılmaz.</summary>
    public bool IsRevoked => RevokedAt.HasValue;
    /// <summary>Tokenın aktif, silinmemiş, iptal edilmemiş ve süresi dolmamış olduğunu hesaplar.</summary>
    public bool IsValid => IsActive && !IsDeleted && !IsExpired && !IsUsed && !IsRevoked;
}

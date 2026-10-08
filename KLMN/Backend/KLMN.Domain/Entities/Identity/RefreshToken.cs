using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Identity;

/// <summary>
/// JWT access token yenileme işlemlerinde kullanılan refresh token kaydını temsil eder.
/// Açık token veritabanında tutulmaz; yalnızca hash değeri saklanır.
/// </summary>
public sealed class RefreshToken : BaseEntity
{
    /// <summary>İlgili kaydın bağlı olduğu kullanıcının benzersiz kimliğidir.</summary>
    public Guid UserId { get; set; }
    /// <summary>Açık token yerine veritabanında saklanan SHA-256 hash değeridir.</summary>
    public string TokenHash { get; set; } = string.Empty;
    /// <summary>Güvenlik tokenının kullanılamaz duruma geleceği UTC zamandır.</summary>
    public DateTime ExpiresAt { get; set; }
    /// <summary>Güvenlik tokenının sunucu tarafından iptal edildiği UTC zamandır.</summary>
    public DateTime? RevokedAt { get; set; }
    /// <summary>Refresh token rotasyonuyla bu kaydın yerine üretilen yeni tokenın kimliğidir.</summary>
    public Guid? ReplacedByTokenId { get; set; }
    /// <summary>Token oluşturma isteğinin geldiği istemci IP adresidir.</summary>
    public string? CreatedByIp { get; set; }
    /// <summary>Token iptal işleminin gerçekleştirildiği istemci IP adresidir.</summary>
    public string? RevokedByIp { get; set; }
    /// <summary>Tokenın neden geçersiz kılındığını açıklayan kayıttır.</summary>
    public string? RevocationReason { get; set; }
    /// <summary>Tokenı üreten istemcinin HTTP User-Agent bilgisidir.</summary>
    public string? UserAgent { get; set; }
    /// <summary>Kullanıcıya gösterilebilen cihaz veya oturum adıdır.</summary>
    public string? DeviceName { get; set; }
    /// <summary>Bu ilişkinin veya güvenlik tokenının ait olduğu kullanıcı navigation alanıdır.</summary>
    public User User { get; set; } = null!;
    /// <summary>Refresh token rotasyonunda yerine geçen tokena ait navigation alanıdır.</summary>
    public RefreshToken? ReplacedByToken { get; set; }

    /// <summary>Geçerli UTC zamanının token bitiş tarihini geçip geçmediğini hesaplar; veritabanına yazılmaz.</summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    /// <summary>Tokenın iptal edilip edilmediğini hesaplar; veritabanına yazılmaz.</summary>
    public bool IsRevoked => RevokedAt.HasValue;
    /// <summary>Tokenın aktif, silinmemiş, iptal edilmemiş ve süresi dolmamış olduğunu hesaplar.</summary>
    public bool IsValid => IsActive && !IsDeleted && !IsRevoked && !IsExpired;
}

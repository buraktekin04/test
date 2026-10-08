using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Identity;

/// <summary>Tek kullanımlık parola sıfırlama token kaydını temsil eder.</summary>
public sealed class PasswordResetToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public User User { get; set; } = null!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsUsed => UsedAt.HasValue;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsValid => IsActive && !IsDeleted && !IsExpired && !IsUsed && !IsRevoked;
}

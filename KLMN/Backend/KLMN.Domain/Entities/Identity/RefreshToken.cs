using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Identity;

/// <summary>
/// Refresh token'ın yalnızca hashlenmiş sunucu tarafı kaydını temsil eder.
/// </summary>
public sealed class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public Guid? ReplacedByTokenId { get; set; }

    public string? CreatedByIp { get; set; }

    public string? RevokedByIp { get; set; }

    public string? RevocationReason { get; set; }

    public string? UserAgent { get; set; }

    public string? DeviceName { get; set; }

    public User User { get; set; } = null!;

    public RefreshToken? ReplacedByToken { get; set; }
}

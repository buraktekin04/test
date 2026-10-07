using KLMN.Domain.Common;
using KLMN.Domain.Entities.Organization;

namespace KLMN.Domain.Entities.Identity;

/// <summary>
/// KLMN uygulama kullanıcısını temsil eder.
/// </summary>
public sealed class User : BaseEntity
{
    public string UserName { get; set; } = string.Empty;

    public string NormalizedUserName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string NormalizedEmail { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public Guid? OrganizationUnitId { get; set; }

    public OrganizationUnit? OrganizationUnit { get; set; }

    public int AccessFailedCount { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public bool IsLocked { get; set; }

    public bool TwoFactorAuthenticationEnabled { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public DateTime? PasswordChangedDate { get; set; }

    public string SecurityStamp { get; set; } = Guid.NewGuid().ToString("N");

    public ICollection<UserRole> UserRoles { get; set; } =
        new HashSet<UserRole>();

    public ICollection<UserPermission> UserPermissions { get; set; } =
        new HashSet<UserPermission>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } =
        new HashSet<RefreshToken>();

    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } =
        new HashSet<PasswordResetToken>();
}

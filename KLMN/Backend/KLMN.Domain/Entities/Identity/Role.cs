using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Identity;

/// <summary>
/// Dinamik kullanıcı rolünü temsil eder.
/// </summary>
public sealed class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsSystemRole { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } =
        new HashSet<UserRole>();

    public ICollection<RolePermission> RolePermissions { get; set; } =
        new HashSet<RolePermission>();
}

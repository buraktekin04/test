using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Identity;

/// <summary>
/// Uygulamadaki atomik permission tanımını temsil eder.
/// </summary>
public sealed class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Module { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int SortOrder { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } =
        new HashSet<RolePermission>();

    public ICollection<UserPermission> UserPermissions { get; set; } =
        new HashSet<UserPermission>();
}

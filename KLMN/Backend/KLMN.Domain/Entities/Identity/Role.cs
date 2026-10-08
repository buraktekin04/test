using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Identity;

/// <summary>Kullanıcılara atanabilen dinamik yetkilendirme rolünü temsil eder.</summary>
public sealed class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

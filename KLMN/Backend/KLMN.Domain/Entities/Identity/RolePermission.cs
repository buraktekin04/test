using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Identity;

/// <summary>Bir role atanmış permission kaydını temsil eder.</summary>
public sealed class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}

using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Identity;

/// <summary>Bir role atanmış permission kaydını temsil eder.</summary>
public sealed class RolePermission : BaseEntity
{
    /// <summary>Atanan rolün benzersiz kimliğidir.</summary>
    public Guid RoleId { get; set; }
    /// <summary>İlişkilendirilen permission kaydının benzersiz kimliğidir.</summary>
    public Guid PermissionId { get; set; }
    /// <summary>Bu ilişkinin ait olduğu rol navigation alanıdır.</summary>
    public Role Role { get; set; } = null!;
    /// <summary>İlişkilendirilen yetki kaydının navigation alanıdır.</summary>
    public Permission Permission { get; set; } = null!;
}

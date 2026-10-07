using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Identity;

/// <summary>
/// Kullanıcıya özel permission override kaydını temsil eder.
/// </summary>
public sealed class UserPermission : BaseEntity
{
    public Guid UserId { get; set; }

    public Guid PermissionId { get; set; }

    /// <summary>
    /// True ise permission kullanıcıya özel olarak verilir.
    /// False ise rolden gelse bile kullanıcıdan kaldırılır.
    /// </summary>
    public bool IsGranted { get; set; }

    public User User { get; set; } = null!;

    public Permission Permission { get; set; } = null!;
}

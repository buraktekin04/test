using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Identity;

/// <summary>Kullanıcı bazlı permission override kaydını temsil eder.</summary>
public sealed class UserPermission : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }

    /// <summary>True izin, false açık ret anlamına gelir.</summary>
    public bool IsGranted { get; set; }

    public User User { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}

using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Identity;

/// <summary>Kullanıcı bazlı permission override kaydını temsil eder.</summary>
public sealed class UserPermission : BaseEntity
{
    /// <summary>İlgili kaydın bağlı olduğu kullanıcının benzersiz kimliğidir.</summary>
    public Guid UserId { get; set; }
    /// <summary>İlişkilendirilen permission kaydının benzersiz kimliğidir.</summary>
    public Guid PermissionId { get; set; }

    /// <summary>True izin, false açık ret anlamına gelir.</summary>
    public bool IsGranted { get; set; }

    /// <summary>Bu ilişkinin veya güvenlik tokenının ait olduğu kullanıcı navigation alanıdır.</summary>
    public User User { get; set; } = null!;
    /// <summary>İlişkilendirilen yetki kaydının navigation alanıdır.</summary>
    public Permission Permission { get; set; } = null!;
}

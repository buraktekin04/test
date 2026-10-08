using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Identity;

/// <summary>Kullanıcı ile rol arasındaki çoktan çoğa ilişkiyi temsil eder.</summary>
public sealed class UserRole : BaseEntity
{
    /// <summary>İlgili kaydın bağlı olduğu kullanıcının benzersiz kimliğidir.</summary>
    public Guid UserId { get; set; }
    /// <summary>Atanan rolün benzersiz kimliğidir.</summary>
    public Guid RoleId { get; set; }
    /// <summary>Bu ilişkinin veya güvenlik tokenının ait olduğu kullanıcı navigation alanıdır.</summary>
    public User User { get; set; } = null!;
    /// <summary>Bu ilişkinin ait olduğu rol navigation alanıdır.</summary>
    public Role Role { get; set; } = null!;
}

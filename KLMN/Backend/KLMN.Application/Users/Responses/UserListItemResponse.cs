namespace KLMN.Application.Users.Responses;

/// <summary>
/// Kullanıcı listeleme ekranı ve Users API'sinin döndürdüğü
/// kullanıcı özet bilgilerini temsil eder.
/// Entity üzerinde bulunan parola hash'i ve güvenlik tokenları
/// gibi gizli bilgiler bu response modeline dahil edilmez.
/// </summary>
public sealed class UserListItemResponse
{
    /// <summary>
    /// Kullanıcının sistemdeki benzersiz GUID kimliğidir.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Kullanıcının oturum açarken kullandığı hesap adıdır.
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// Kullanıcının adıdır.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Kullanıcının soyadıdır.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Liste ekranında gösterilecek ad ve soyadın birleştirilmiş halidir.
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Kullanıcının sistemde kayıtlı e-posta adresidir.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Kullanıcıya ait isteğe bağlı iletişim telefon numarasıdır.
    /// </summary>
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Kullanıcının bağlı olduğu organizasyon biriminin kimliğidir.
    /// Birime bağlı olmayan teknik kullanıcılar için boş olabilir.
    /// </summary>
    public Guid? OrganizationUnitId { get; init; }

    /// <summary>
    /// Bağlı olunan organizasyon biriminin teknik kodudur.
    /// </summary>
    public string? OrganizationUnitCode { get; init; }

    /// <summary>
    /// Bağlı olunan organizasyon biriminin ekranda gösterilen adıdır.
    /// </summary>
    public string? OrganizationUnitName { get; init; }

    /// <summary>
    /// Kullanıcının aktif rol ilişkilerinden elde edilen rol kodlarını içerir.
    /// </summary>
    public IReadOnlyCollection<string> Roles { get; init; } = [];

    /// <summary>
    /// Kullanıcının işlemlerde kullanılabilir durumda olup olmadığını gösterir.
    /// Pasif olması kaydın silindiği anlamına gelmez.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Hesabın geçici veya süresiz olarak kilitlenip kilitlenmediğini belirtir.
    /// </summary>
    public bool IsLocked { get; init; }

    /// <summary>
    /// Kullanıcı kaydının oluşturulduğu UTC tarih ve saattir.
    /// </summary>
    public DateTime CreatedDate { get; init; }

    /// <summary>
    /// PostgreSQL xmin üzerinden yönetilen optimistic concurrency
    /// satır versiyonudur. Eşzamanlı güncellemelerde çakışma kontrolünde kullanılır.
    /// </summary>
    public uint Version { get; init; }
}

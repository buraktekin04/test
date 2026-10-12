using KLMN.Application.Users.Responses;
using MediatR;

namespace KLMN.Application.Users.Commands.UpdateUser;

/// <summary>
/// Kullanıcı profilini eşzamanlılık versiyonu ile güncelleme isteğidir.
/// Parola, rol ve özel yetkiler bu komutla değiştirilemez.
/// </summary>
public sealed record UpdateUserCommand : IRequest<UserMutationResponse>
{
    /// <summary>Güncellenecek kullanıcının benzersiz kimliğidir.</summary>
    public Guid Id { get; init; }
    /// <summary>İstemcinin son okuduğu PostgreSQL xmin değeridir.</summary>
    public uint Version { get; init; }
    /// <summary>Benzersiz giriş kullanıcı adıdır.</summary>
    public required string UserName { get; init; }
    /// <summary>Güncellenecek addır.</summary>
    public required string FirstName { get; init; }
    /// <summary>Güncellenecek soyaddır.</summary>
    public required string LastName { get; init; }
    /// <summary>Benzersiz e-posta adresidir.</summary>
    public required string Email { get; init; }
    /// <summary>İsteğe bağlı telefon numarasıdır.</summary>
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Kullanıcının görev yeri olarak seçilen organizasyon biriminin GUID'idir.
    /// Mevcut şemayla uyumluluk için boş olabilir; rol ve yetki ataması değildir.
    /// </summary>
    public Guid? OrganizationUnitId { get; init; }

    /// <summary>true ise mesai dışı giriş engellenir, false ise mesai dışı erişime izin verilir.</summary>
    public bool ClockLimit { get; init; } = true;

    /// <summary>
    /// Yönetici tarafından uygulanan manuel kilit durumu. True kullanıcıyı
    /// login, refresh ve aktif JWT isteklerinde erişime kapatır.
    /// </summary>
    // Geriye dönük uyumluluk: eski istemciler bu alanı göndermediğinde
    // mevcut kilit durumunu istemeden sıfırlamamak için nullable bırakılır.
    public bool? IsLocked { get; init; }

    /// <summary>Hesabın kullanılmaya başlanabileceği UTC anıdır (dahil, null sınırsız).</summary>
    public DateTime? ValidFrom { get; init; }

    /// <summary>Hesabın kullanılabileceği son UTC sınırıdır (hariç, null sınırsız).</summary>
    public DateTime? ValidTo { get; init; }
}

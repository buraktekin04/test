using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Identity;

/// <summary>
/// Kullanıcıya bağlı sözleşme/bilgilendirme PDF kayıtlarının meta verisidir.
/// PDF'nin binary içeriği PostgreSQL'e yazılmaz; özel sunucu klasöründe tutulur.
/// </summary>
public sealed class UserAgreementPdf : BaseEntity
{
    /// <summary>PDF'nin ilişkili olduğu kullanıcı GUID değeridir.</summary>
    public Guid UserId { get; set; }

    /// <summary>PDF'nin bağlı olduğu kullanıcı navigation alanıdır.</summary>
    public User User { get; set; } = null!;

    /// <summary>Yükleme anında gelen ve indirme adında gösterilecek özgün dosya ismidir.</summary>
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// Diskteki rastgele üretilmiş güvenli dosya anahtarıdır.
    /// Fiziksel kök dizin appsettings DocumentStorage bölümündedir; DB'ye mutlak path yazılmaz.
    /// </summary>
    public string RelativePath { get; set; } = string.Empty;

    /// <summary>PDF'nin sunucuda saklanan byte uzunluğudur.</summary>
    public long SizeBytes { get; set; }

    /// <summary>PDF indirme sırasında kullanılan MIME türüdür.</summary>
    public string ContentType { get; set; } = "application/pdf";
}

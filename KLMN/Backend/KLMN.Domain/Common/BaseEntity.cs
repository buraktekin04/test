namespace KLMN.Domain.Common;

/// <summary>
/// Sistemde kalıcı olarak saklanan entity'lerin ortak temel sınıfıdır.
/// Kimlik, audit, aktiflik, soft delete ve optimistic concurrency
/// bilgilerini merkezi olarak taşır.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Kaydın sistem genelindeki benzersiz kimliğidir.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Kaydın oluşturulduğu UTC tarih ve saattir.
    /// AuditableEntitySaveChangesInterceptor tarafından doldurulur.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>Kaydı oluşturan kullanıcı kimliğidir. Sistem işlemlerinde boş olabilir.</summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>Kaydın son güncellendiği UTC tarih ve saattir.</summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>Kaydı son güncelleyen kullanıcının kimliğidir.</summary>
    public Guid? UpdatedBy { get; set; }

    /// <summary>İş süreçlerinde kullanılabilirliğini belirtir; pasiflik silinmek değildir.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Kaydın soft delete ile silinmiş olarak işaretlenip işaretlenmediğidir.</summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>Soft delete işleminin UTC tarih ve saatidir.</summary>
    public DateTime? DeletedDate { get; set; }

    /// <summary>Soft delete işlemini gerçekleştiren kullanıcının kimliğidir.</summary>
    public Guid? DeletedBy { get; set; }

    /// <summary>
    /// Optimistic concurrency kontrolünde kullanılan satır versiyonudur.
    /// PostgreSQL'de fiziksel Version kolonu oluşturulmaz; Npgsql bu alanı
    /// PostgreSQL'in xmin sistem kolonuna eşler.
    /// </summary>
    public uint Version { get; set; }
}

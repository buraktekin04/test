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

    /// <summary>Kaydın oluşturulduğu UTC tarih ve saattir.</summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>Kaydı oluşturan kullanıcının kimliğidir.</summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>Kaydın son güncellendiği UTC tarih ve saattir.</summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>Kaydı son güncelleyen kullanıcının kimliğidir.</summary>
    public Guid? UpdatedBy { get; set; }

    /// <summary>Kaydın iş süreçlerinde kullanılabilir durumda olup olmadığını belirtir.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Kaydın soft delete ile silinmiş olup olmadığını belirtir.</summary>
    public bool IsDeleted { get; set; }

    /// <summary>Kaydın soft delete tarihidir.</summary>
    public DateTime? DeletedDate { get; set; }

    /// <summary>Kaydı soft delete yapan kullanıcının kimliğidir.</summary>
    public Guid? DeletedBy { get; set; }

    /// <summary>
    /// Optimistic concurrency kontrolünde kullanılan satır versiyonudur.
    /// PostgreSQL tarafında fiziksel Version kolonu oluşturulmaz;
    /// Npgsql tarafından xmin sistem kolonuna map edilir.
    /// </summary>
    public uint Version { get; set; }
}

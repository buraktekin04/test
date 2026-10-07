namespace KLMN.Domain.Common;

/// <summary>
/// KLMN domain entity'lerinin ortak audit, durum ve optimistic concurrency
/// alanlarını tanımlayan temel entity sınıfıdır.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Kaydın benzersiz kimliğidir.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Kaydın oluşturulma tarihidir.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Kaydı oluşturan kullanıcının kimliğidir.
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// Kaydın son güncellenme tarihidir.
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Kaydı son güncelleyen kullanıcının kimliğidir.
    /// </summary>
    public Guid? UpdatedBy { get; set; }

    /// <summary>
    /// Kaydın aktif durumda olup olmadığını belirtir.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Kaydın soft delete ile silinip silinmediğini belirtir.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Kaydın silinme tarihidir.
    /// </summary>
    public DateTime? DeletedDate { get; set; }

    /// <summary>
    /// Kaydı silen kullanıcının kimliğidir.
    /// </summary>
    public Guid? DeletedBy { get; set; }

    /// <summary>
    /// PostgreSQL xmin sistem kolonu üzerinden optimistic concurrency
    /// kontrolünde kullanılan sürüm bilgisidir.
    /// </summary>
    public uint Version { get; set; }
}

namespace KLMN.Domain.Entities.Auditing;

/// <summary>
/// Tanımlama kayıtlarında gerçekleşen işlemlerin değiştirilemez geçmişidir.
/// BaseEntity kullanılmaz: AuditLog tekrar denetlenmez veya soft-delete edilmez.
/// </summary>
public sealed class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime OccurredAt { get; set; }
    public Guid? ActorUserId { get; set; }
    public string? ActorUserName { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    /// <summary>İlişki kayıtlarının ana tanımlama türü (User veya Role).</summary>
    public string? RelatedEntityType { get; set; }
    /// <summary>İlişki kaydının bağlı olduğu ana tanımlama anahtarıdır.</summary>
    public Guid? RelatedEntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    /// <summary>Yalnızca izinli alanların önceki ve sonraki değerlerini içeren JSON.</summary>
    public string ChangesJson { get; set; } = "{}";
    public string? IpAddress { get; set; }
}

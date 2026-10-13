namespace KLMN.Application.Auditing.Responses;

/// <summary>Audit geçmişinin sadece okunabilir istemci sözleşmesidir.</summary>
public sealed record AuditLogResponse(
    Guid Id,
    DateTime OccurredAt,
    Guid? ActorUserId,
    string? ActorUserName,
    string EntityType,
    Guid EntityId,
    string Action,
    string ChangesJson,
    string? IpAddress)
{
    /// <summary>
    /// RoleId/PermissionId gibi ilişkisel GUID'lerin ad ve kodla gösterilen
    /// salt okunur karşılığıdır. Ham ChangesJson değişmez.
    /// </summary>
    public string? DisplayChangesJson { get; init; }
}

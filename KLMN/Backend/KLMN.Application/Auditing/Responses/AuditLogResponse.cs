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
    string? IpAddress);

using KLMN.Application.Auditing.Responses;
using KLMN.Application.Common.Models;
using MediatR;

namespace KLMN.Application.Auditing.Queries.GetAuditLogs;

/// <summary>Genel geçmiş ve tek kayda ait geçmiş için aynı filtreli sayfalama isteğidir.</summary>
public sealed record GetAuditLogsQuery(
    string? EntityType = null,
    Guid? EntityId = null,
    string? Action = null,
    Guid? ActorUserId = null,
    DateTime? From = null,
    DateTime? To = null,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PagedResult<AuditLogResponse>>;

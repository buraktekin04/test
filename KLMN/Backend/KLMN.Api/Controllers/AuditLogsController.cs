using KLMN.Api.Authorization;
using KLMN.Application.Auditing.Queries.GetAuditLogs;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Tanımlama geçmişini güvenli şekilde yalnızca okunabilir olarak sunar.</summary>
[ApiController]
[Route("api/audit-logs")]
public sealed class AuditLogsController(ISender sender) : ControllerBase
{
    /// <summary>İsteğe bağlı entityType+entityId ile satır geçmişi, aksi halde genel geçmişi döndürür.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.AuditLogs.View)]
    public async Task<IActionResult> Get([FromQuery] GetAuditLogsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));
}

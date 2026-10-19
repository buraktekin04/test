using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.ConnectedDecisions.Commands.CreateHcConnectedDecisionDef;
using KLMN.Application.ConnectedDecisions.Commands.UpdateHcConnectedDecisionDef;
using KLMN.Application.ConnectedDecisions.Commands.ChangeStatusHcConnectedDecisionDef;
using KLMN.Application.ConnectedDecisions.Commands.DeleteHcConnectedDecisionDef;
using KLMN.Application.ConnectedDecisions.Queries.GetHcConnectedDecisionDefs;
using KLMN.Application.ConnectedDecisions.Queries.GetHcConnectedDecisionDefById;
using KLMN.Application.ConnectedDecisions.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Bağlı Kurul Karar Tanımları CRUD, Excel ve permission API'si.</summary>
[ApiController]
[Route("api/connected-decisions")]
public sealed class ConnectedDecisionsController(ISender sender, IExcelExportService excel) : ControllerBase
{
    /// <summary>Yetkili kullanıcıya filtreli, sayfalı liste.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.ConnectedDecisions.View, PermissionCodes.ConnectedDecisions.Query,
        PermissionCodes.ConnectedDecisions.Create, PermissionCodes.ConnectedDecisions.Update,
        PermissionCodes.ConnectedDecisions.Delete)]
    public async Task<IActionResult> List([FromQuery] GetHcConnectedDecisionDefsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>En çok 5000 adet sonucu sorgu yetkisi ile Excel'e dönüştürür.</summary>
    [HttpGet("export")]
    [HasPermission(PermissionCodes.ConnectedDecisions.Query)]
    public async Task<IActionResult> Export([FromQuery] GetHcConnectedDecisionDefsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails {
                Status = 400, Title = "Excel aktarım sınırı aşıldı.",
                Detail = "En fazla 5.000 kayıt indirilebilir; filtreleri daraltınız."
            });

        var rows = new List<HcConnectedDecisionDefResponse>(first.Items);
        var pages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var page = 2; page <= pages; page++)
        {
            var batch = await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct);
            rows.AddRange(batch.Items);
        }
        var columns = new ExcelColumn<HcConnectedDecisionDefResponse>[]
        {
            new("Kod", x => x.Code, Width: 22),
            new("Bağlı Kurul Kararı Adı", x => x.Name, Width: 38),
            new("Geçerli Başlangıç", x => x.ValidFrom, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Geçerli Bitiş", x => x.ValidTo, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 18),
            new("Oluşturulma", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 24)
        };
        return File(excel.Export(rows, columns, "Bağlı Kurul Karar Tanımları"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "connected-decisions_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".xlsx");
    }

    /// <summary>Detayını ve eşzamanlılık sürümünü döndürür.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.ConnectedDecisions.View, PermissionCodes.ConnectedDecisions.Query,
        PermissionCodes.ConnectedDecisions.Create, PermissionCodes.ConnectedDecisions.Update,
        PermissionCodes.ConnectedDecisions.Delete)]
    public async Task<IActionResult> ById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetHcConnectedDecisionDefByIdQuery(id), ct));

    /// <summary>Bağlı kurul kararı ekler.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.ConnectedDecisions.Create)]
    public async Task<IActionResult> Create([FromBody] CreateHcConnectedDecisionDefCommand request, CancellationToken ct)
    {
        var item = await sender.Send(request, ct);
        return CreatedAtAction(nameof(ById), new { id = item.Id }, item);
    }

    /// <summary>Güncel xmin ile bağlı kurul kararı günceller.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.ConnectedDecisions.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHcConnectedDecisionDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>BaseEntity.IsActive üzerinden durum değiştirir.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.ConnectedDecisions.Update)]
    public async Task<IActionResult> Status(Guid id, [FromBody] ChangeStatusHcConnectedDecisionDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>Optimistic concurrency ile soft-delete.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.ConnectedDecisions.Delete)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteHcConnectedDecisionDefCommand(id, version), ct);
        return NoContent();
    }
}

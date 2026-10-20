using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.Procedures.Commands.CreateProcedureDef;
using KLMN.Application.Procedures.Commands.UpdateProcedureDef;
using KLMN.Application.Procedures.Commands.ChangeStatusProcedureDef;
using KLMN.Application.Procedures.Commands.DeleteProcedureDef;
using KLMN.Application.Procedures.Queries.GetProcedureDefs;
using KLMN.Application.Procedures.Queries.GetProcedureDefById;
using KLMN.Application.Procedures.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Tetkik Tanımları CRUD, Excel ve permission API'si.</summary>
[ApiController]
[Route("api/procedures")]
public sealed class ProceduresController(ISender sender, IExcelExportService excel) : ControllerBase
{
    /// <summary>Yetkili kullanıcıya filtreli, sayfalı liste.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.Procedures.View, PermissionCodes.Procedures.Query,
        PermissionCodes.Procedures.Create, PermissionCodes.Procedures.Update,
        PermissionCodes.Procedures.Delete)]
    public async Task<IActionResult> List([FromQuery] GetProcedureDefsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>En çok 5000 adet sonucu sorgu yetkisi ile Excel'e dönüştürür.</summary>
    [HttpGet("export")]
    [HasPermission(PermissionCodes.Procedures.Query)]
    public async Task<IActionResult> Export([FromQuery] GetProcedureDefsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails {
                Status = 400, Title = "Excel aktarım sınırı aşıldı.",
                Detail = "En fazla 5.000 kayıt indirilebilir; filtreleri daraltınız."
            });

        var rows = new List<ProcedureDefResponse>(first.Items);
        var pages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var page = 2; page <= pages; page++)
        {
            var batch = await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct);
            rows.AddRange(batch.Items);
        }
        var columns = new ExcelColumn<ProcedureDefResponse>[]
        {
            new("Kod", x => x.Code, Width: 22),
            new("Tetkik Adı", x => x.Name, Width: 38),
            new("Geçerli Başlangıç", x => x.ValidFrom, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Geçerli Bitiş", x => x.ValidTo, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 18),
            new("Oluşturulma", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 24)
        };
        return File(excel.Export(rows, columns, "Tetkik Tanımları"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "procedures_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".xlsx");
    }

    /// <summary>Detayını ve eşzamanlılık sürümünü döndürür.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.Procedures.View, PermissionCodes.Procedures.Query,
        PermissionCodes.Procedures.Create, PermissionCodes.Procedures.Update,
        PermissionCodes.Procedures.Delete)]
    public async Task<IActionResult> ById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetProcedureDefByIdQuery(id), ct));

    /// <summary>Tetkik ekler.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.Procedures.Create)]
    public async Task<IActionResult> Create([FromBody] CreateProcedureDefCommand request, CancellationToken ct)
    {
        var item = await sender.Send(request, ct);
        return CreatedAtAction(nameof(ById), new { id = item.Id }, item);
    }

    /// <summary>Güncel xmin ile tetkik günceller.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.Procedures.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProcedureDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>BaseEntity.IsActive üzerinden durum değiştirir.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.Procedures.Update)]
    public async Task<IActionResult> Status(Guid id, [FromBody] ChangeStatusProcedureDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>Optimistic concurrency ile soft-delete.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.Procedures.Delete)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteProcedureDefCommand(id, version), ct);
        return NoContent();
    }
}

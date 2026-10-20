using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.Ranks.Commands.CreateRankDef;
using KLMN.Application.Ranks.Commands.UpdateRankDef;
using KLMN.Application.Ranks.Commands.ChangeStatusRankDef;
using KLMN.Application.Ranks.Commands.DeleteRankDef;
using KLMN.Application.Ranks.Queries.GetRankDefs;
using KLMN.Application.Ranks.Queries.GetRankDefById;
using KLMN.Application.Ranks.Responses;
using KLMN.Application.Ranks.Queries.GetRankOptions;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Rütbe Tanımları CRUD, Excel ve permission API'si.</summary>
[ApiController]
[Route("api/ranks")]
public sealed class RanksController(ISender sender, IExcelExportService excel) : ControllerBase
{
    /// <summary>Yetkili kullanıcıya filtreli, sayfalı liste.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.Ranks.View, PermissionCodes.Ranks.Query,
        PermissionCodes.Ranks.Create, PermissionCodes.Ranks.Update,
        PermissionCodes.Ranks.Delete)]
    public async Task<IActionResult> List([FromQuery] GetRankDefsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>En çok 5000 adet sonucu sorgu yetkisi ile Excel'e dönüştürür.</summary>
    [HttpGet("export")]
    [HasPermission(PermissionCodes.Ranks.Query)]
    public async Task<IActionResult> Export([FromQuery] GetRankDefsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails {
                Status = 400, Title = "Excel aktarım sınırı aşıldı.",
                Detail = "En fazla 5.000 kayıt indirilebilir; filtreleri daraltınız."
            });

        var rows = new List<RankDefResponse>(first.Items);
        var pages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var page = 2; page <= pages; page++)
        {
            var batch = await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct);
            rows.AddRange(batch.Items);
        }
        var columns = new ExcelColumn<RankDefResponse>[]
        {
            new("Kod", x => x.Code, Width: 22),
            new("Rütbe Adı", x => x.Name, Width: 38),
            new("Kısa Ad", x => x.ShortName, Width: 20),
            new("Tür (Sayısal)", x => x.Type, Width: 18),
            new("Rütbe Grubu", x => x.RankGroupName, Width: 26),
            new("Geçerli Başlangıç", x => x.ValidFrom, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Geçerli Bitiş", x => x.ValidTo, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 18),
            new("Oluşturulma", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 24)
        };
        return File(excel.Export(rows, columns, "Rütbe Tanımları"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "ranks_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".xlsx");
    }

    /// <summary>Rütbe yetkisiyle aktif rütbe gruplarını, düzenlemede seçili pasif grubu getirir.</summary>
    [HttpGet("options")]
    [HasPermission(PermissionCodes.Ranks.View, PermissionCodes.Ranks.Query,
        PermissionCodes.Ranks.Create, PermissionCodes.Ranks.Update,
        PermissionCodes.Ranks.Delete)]
    public async Task<IActionResult> Options([FromQuery] Guid? selectedRankGroupId, CancellationToken ct) =>
        Ok(await sender.Send(new GetRankOptionsQuery(selectedRankGroupId), ct));

    /// <summary>Detayını ve eşzamanlılık sürümünü döndürür.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.Ranks.View, PermissionCodes.Ranks.Query,
        PermissionCodes.Ranks.Create, PermissionCodes.Ranks.Update,
        PermissionCodes.Ranks.Delete)]
    public async Task<IActionResult> ById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetRankDefByIdQuery(id), ct));

    /// <summary>Rütbe ekler.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.Ranks.Create)]
    public async Task<IActionResult> Create([FromBody] CreateRankDefCommand request, CancellationToken ct)
    {
        var item = await sender.Send(request, ct);
        return CreatedAtAction(nameof(ById), new { id = item.Id }, item);
    }

    /// <summary>Güncel xmin ile rütbe günceller.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.Ranks.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRankDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>BaseEntity.IsActive üzerinden durum değiştirir.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.Ranks.Update)]
    public async Task<IActionResult> Status(Guid id, [FromBody] ChangeStatusRankDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>Optimistic concurrency ile soft-delete.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.Ranks.Delete)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteRankDefCommand(id, version), ct);
        return NoContent();
    }
}

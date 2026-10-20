using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.RankGroups.Commands.CreateRankGroupDef;
using KLMN.Application.RankGroups.Commands.UpdateRankGroupDef;
using KLMN.Application.RankGroups.Commands.ChangeStatusRankGroupDef;
using KLMN.Application.RankGroups.Commands.DeleteRankGroupDef;
using KLMN.Application.RankGroups.Queries.GetRankGroupDefs;
using KLMN.Application.RankGroups.Queries.GetRankGroupDefById;
using KLMN.Application.RankGroups.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Rütbe Grubu Tanımları CRUD, Excel ve permission API'si.</summary>
[ApiController]
[Route("api/rank-groups")]
public sealed class RankGroupsController(ISender sender, IExcelExportService excel) : ControllerBase
{
    /// <summary>Yetkili kullanıcıya filtreli, sayfalı liste.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.RankGroups.View, PermissionCodes.RankGroups.Query,
        PermissionCodes.RankGroups.Create, PermissionCodes.RankGroups.Update,
        PermissionCodes.RankGroups.Delete)]
    public async Task<IActionResult> List([FromQuery] GetRankGroupDefsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>En çok 5000 adet sonucu sorgu yetkisi ile Excel'e dönüştürür.</summary>
    [HttpGet("export")]
    [HasPermission(PermissionCodes.RankGroups.Query)]
    public async Task<IActionResult> Export([FromQuery] GetRankGroupDefsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails {
                Status = 400, Title = "Excel aktarım sınırı aşıldı.",
                Detail = "En fazla 5.000 kayıt indirilebilir; filtreleri daraltınız."
            });

        var rows = new List<RankGroupDefResponse>(first.Items);
        var pages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var page = 2; page <= pages; page++)
        {
            var batch = await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct);
            rows.AddRange(batch.Items);
        }
        var columns = new ExcelColumn<RankGroupDefResponse>[]
        {
            new("Kod", x => x.Code, Width: 22),
            new("Rütbe Grubu Adı", x => x.Name, Width: 38),
            new("Kısa Ad", x => x.ShortName, Width: 18),
            new("Geçerli Başlangıç", x => x.ValidFrom, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Geçerli Bitiş", x => x.ValidTo, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 18),
            new("Oluşturulma", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 24)
        };
        return File(excel.Export(rows, columns, "Rütbe Grubu Tanımları"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "rank-groups_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".xlsx");
    }

    /// <summary>Detayını ve eşzamanlılık sürümünü döndürür.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.RankGroups.View, PermissionCodes.RankGroups.Query,
        PermissionCodes.RankGroups.Create, PermissionCodes.RankGroups.Update,
        PermissionCodes.RankGroups.Delete)]
    public async Task<IActionResult> ById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetRankGroupDefByIdQuery(id), ct));

    /// <summary>Rütbe Grubu ekler.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.RankGroups.Create)]
    public async Task<IActionResult> Create([FromBody] CreateRankGroupDefCommand request, CancellationToken ct)
    {
        var item = await sender.Send(request, ct);
        return CreatedAtAction(nameof(ById), new { id = item.Id }, item);
    }

    /// <summary>Güncel xmin ile rütbe grubu günceller.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.RankGroups.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRankGroupDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>BaseEntity.IsActive üzerinden durum değiştirir.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.RankGroups.Update)]
    public async Task<IActionResult> Status(Guid id, [FromBody] ChangeStatusRankGroupDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>Optimistic concurrency ile soft-delete.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.RankGroups.Delete)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteRankGroupDefCommand(id, version), ct);
        return NoContent();
    }
}

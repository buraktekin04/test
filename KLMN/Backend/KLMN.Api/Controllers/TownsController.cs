using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.Towns.Commands.CreateTownDef;
using KLMN.Application.Towns.Commands.UpdateTownDef;
using KLMN.Application.Towns.Commands.ChangeStatusTownDef;
using KLMN.Application.Towns.Commands.DeleteTownDef;
using KLMN.Application.Towns.Queries.GetTownDefs;
using KLMN.Application.Towns.Queries.GetTownDefById;
using KLMN.Application.Towns.Responses;
using KLMN.Application.Towns.Queries.GetTownCityOptions;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>İlçe Tanımları için permission korumalı CQRS/Excel API uç noktaları.</summary>
[ApiController]
[Route("api/towns")]
public sealed class TownsController(ISender sender, IExcelExportService excel) : ControllerBase
{
    [HttpGet]
    [HasPermission(PermissionCodes.Towns.View, PermissionCodes.Towns.Query,
        PermissionCodes.Towns.Create, PermissionCodes.Towns.Update, PermissionCodes.Towns.Delete)]
    public async Task<IActionResult> List([FromQuery] GetTownDefsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    [HttpGet("export")]
    [HasPermission(PermissionCodes.Towns.Query)]
    public async Task<IActionResult> Export([FromQuery] GetTownDefsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber=1, PageSize=pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails { Status=400, Title="Excel aktarım sınırı aşıldı.",
                Detail="En fazla 5.000 kayıt indirilebilir; filtreleri daraltınız." });
        var rows = new List<TownDefResponse>(first.Items);
        var pages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var p = 2; p <= pages; p++)
        {
            var batch = await sender.Send(query with { PageNumber=p, PageSize=pageSize }, ct);
            rows.AddRange(batch.Items);
        }
        var columns = new ExcelColumn<TownDefResponse>[]
        {
            new("Kod", x => x.Code, Width: 22),
            new("İlçe Adı", x => x.Name, Width: 36),
            new("Harici Kod", x => x.ExternalCode, Width: 24),
            new("Şehir", x => x.CityName, Width: 32),
            new("MERNİS Kodu", x => x.MernisCode, Width: 20),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 16),
            new("Oluşturulma", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25)
        };
        return File(excel.Export(rows, columns, "İlçe Tanımları"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "towns_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".xlsx");
    }

    [HttpGet("city-options")]
    [HasPermission(PermissionCodes.Towns.View, PermissionCodes.Towns.Query,
        PermissionCodes.Towns.Create, PermissionCodes.Towns.Update, PermissionCodes.Towns.Delete)]
    public async Task<IActionResult> GetCityOptions(
        [FromQuery] Guid? selectedCityId, CancellationToken ct) =>
        Ok(await sender.Send(new GetTownCityOptionsQuery(selectedCityId), ct));
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.Towns.View, PermissionCodes.Towns.Query,
        PermissionCodes.Towns.Create, PermissionCodes.Towns.Update, PermissionCodes.Towns.Delete)]
    public async Task<IActionResult> ById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetTownDefByIdQuery(id), ct));

    [HttpPost]
    [HasPermission(PermissionCodes.Towns.Create)]
    public async Task<IActionResult> Create([FromBody] CreateTownDefCommand request, CancellationToken ct)
    {
        var item = await sender.Send(request, ct);
        return CreatedAtAction(nameof(ById), new { id=item.Id }, item);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.Towns.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTownDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.Towns.Update)]
    public async Task<IActionResult> Status(Guid id, [FromBody] ChangeStatusTownDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.Towns.Delete)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteTownDefCommand(id, version), ct);
        return NoContent();
    }
}

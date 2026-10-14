using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.PurchaseNumbers.Commands.CreatePurchaseNumberDef;
using KLMN.Application.PurchaseNumbers.Commands.UpdatePurchaseNumberDef;
using KLMN.Application.PurchaseNumbers.Commands.ChangePurchaseNumberDefStatus;
using KLMN.Application.PurchaseNumbers.Commands.DeletePurchaseNumberDef;
using KLMN.Application.PurchaseNumbers.Queries.GetPurchaseNumberDefs;
using KLMN.Application.PurchaseNumbers.Queries.GetPurchaseNumberDefById;
using KLMN.Application.PurchaseNumbers.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Alım Numarası Tanımları API: CRUD, filtreler, Excel ve durum.</summary>
[ApiController]
[Route("api/purchase-numbers")]
public sealed class PurchaseNumbersController(ISender sender, IExcelExportService excel)
    : ControllerBase
{
    [HttpGet]
    [HasPermission(PermissionCodes.PurchaseNumbers.View,
        PermissionCodes.PurchaseNumbers.Query, PermissionCodes.PurchaseNumbers.Create,
        PermissionCodes.PurchaseNumbers.Update, PermissionCodes.PurchaseNumbers.Delete)]
    public async Task<IActionResult> GetList(
        [FromQuery] GetPurchaseNumberDefsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    [HttpGet("export")]
    [HasPermission(PermissionCodes.PurchaseNumbers.Query)]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    public async Task<IActionResult> Export(
        [FromQuery] GetPurchaseNumberDefsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Excel kayıt limiti aşıldı.",
                Detail = "En fazla 5.000 alım numarası Excel'e aktarılabilir. Filtreleri daraltınız."
            });
        var rows = new List<PurchaseNumberDefResponse>(first.Items);
        var pageCount = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var page = 2; page <= pageCount; page++)
        {
            var batch = await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct);
            rows.AddRange(batch.Items);
        }
        var columns = new ExcelColumn<PurchaseNumberDefResponse>[]
        {
            new("Kod", x => x.Code, Width: 20),
            new("Alım Adı", x => x.Name, Width: 36),
            new("Temin Tanım Adı", x => x.SupplyDescriptionName, Width: 48),
            new("Başlangıç", x => x.ValidFrom, NumberFormat: "dd.MM.yyyy HH:mm", Width: 24),
            new("Bitiş", x => x.ValidTo, NumberFormat: "dd.MM.yyyy HH:mm", Width: 24),
            new("Sağlık Raporu Bitişi", x => x.HealthReportEndDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 15),
            new("Oluşturulma", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 24)
        };
        return File(excel.Export(rows, columns, "Alım Numaraları"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Alim_Numarasi_Tanimlari_{DateTime.UtcNow:yyyyMMdd_HHmm}.xlsx");
    }

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.PurchaseNumbers.View,
        PermissionCodes.PurchaseNumbers.Query, PermissionCodes.PurchaseNumbers.Create,
        PermissionCodes.PurchaseNumbers.Update, PermissionCodes.PurchaseNumbers.Delete)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetPurchaseNumberDefByIdQuery(id), ct));

    [HttpPost]
    [HasPermission(PermissionCodes.PurchaseNumbers.Create)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePurchaseNumberDefCommand command, CancellationToken ct)
    {
        var created = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.PurchaseNumbers.Update)]
    public async Task<IActionResult> Update(Guid id,
        [FromBody] UpdatePurchaseNumberDefCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("Route ve alım numarası kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(command, ct));
    }

    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.PurchaseNumbers.Update)]
    public async Task<IActionResult> ChangeStatus(Guid id,
        [FromBody] ChangePurchaseNumberDefStatusCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("Route ve alım numarası kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(command, ct));
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.PurchaseNumbers.Delete)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeletePurchaseNumberDefCommand(id, version), ct);
        return NoContent();
    }
}

using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.ForcesCommands.Commands.CreateForcesCommandDef;
using KLMN.Application.ForcesCommands.Commands.UpdateForcesCommandDef;
using KLMN.Application.ForcesCommands.Commands.ChangeForcesCommandDefStatus;
using KLMN.Application.ForcesCommands.Commands.DeleteForcesCommandDef;
using KLMN.Application.ForcesCommands.Queries.GetForcesCommandDefs;
using KLMN.Application.ForcesCommands.Queries.GetForcesCommandDefById;
using KLMN.Application.ForcesCommands.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Kuvvet komutanlığı Tanımları: CRUD, geçerlilik ve Excel endpoint'leri.</summary>
[ApiController]
[Route("api/forces-commands")]
public sealed class ForcesCommandsController(ISender sender, IExcelExportService excel)
    : ControllerBase
{
    /// <summary>Kuvvet komutanlıklarını arama ve aktiflik filtreleriyle listeler.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.ForcesCommands.View,
        PermissionCodes.ForcesCommands.Query,
        PermissionCodes.ForcesCommands.Create,
        PermissionCodes.ForcesCommands.Update,
        PermissionCodes.ForcesCommands.Delete)]
    public async Task<IActionResult> GetForcesCommands(
        [FromQuery] GetForcesCommandDefsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>Aranmış ve filtrelenmiş tüm kayıtları ortak XLSX servisiyle dışa aktarır.</summary>
    [HttpGet("export")]
    [HasPermission(PermissionCodes.ForcesCommands.Query)]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    public async Task<IActionResult> ExportForcesCommands(
        [FromQuery] GetForcesCommandDefsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Excel kayıt limiti aşıldı.",
                Detail = "En fazla 5.000 kuvvet komutanlığı dışa aktarılabilir."
            });

        var items = new List<ForcesCommandDefResponse>(first.Items);
        var totalPages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var page = 2; page <= totalPages; page++)
        {
            var batch = await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct);
            items.AddRange(batch.Items);
        }

        var columns = new ExcelColumn<ForcesCommandDefResponse>[]
        {
            new("Kuvvet Komutanlığı", x => x.Name, Width: 38),
            new("Kod", x => x.Code, Width: 22),
            new("Geçerli Başlangıç", x => x.ValidFrom, NumberFormat: "dd.mm.yyyy hh:mm", Width: 22),
            new("Geçerli Bitiş", x => x.ValidTo, NumberFormat: "dd.mm.yyyy hh:mm", Width: 22),
            new("Bağlı Birlik", x => x.MilitaryUnitCount, Width: 20),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 16),
            new("Oluşturulma (UTC)", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 26)
        };

        return File(excel.Export(items, columns, "Kuvvet Komutanlıkları"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Kuvvet_Komutanligi_Tanimlari_{DateTime.UtcNow:yyyyMMdd_HHmm}.xlsx");
    }

    /// <summary>Mevcut pasif kayıtlar dahil kimlik üzerinden güncel birimi getirir.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.ForcesCommands.View,
        PermissionCodes.ForcesCommands.Query,
        PermissionCodes.ForcesCommands.Create,
        PermissionCodes.ForcesCommands.Update,
        PermissionCodes.ForcesCommands.Delete)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetForcesCommandDefByIdQuery(id), ct));

    /// <summary>Yeni kuvvet komutanlığı oluşturur.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.ForcesCommands.Create)]
    public async Task<IActionResult> Create(
        [FromBody] CreateForcesCommandDefCommand command, CancellationToken ct)
    {
        var created = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Güncel xmin ile kuvvet komutanlığı bilgilerini değiştirir.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.ForcesCommands.Update)]
    public async Task<IActionResult> Update(Guid id,
        [FromBody] UpdateForcesCommandDefCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("Route ve kuvvet komutanlığı kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(command, ct));
    }

    /// <summary>Aktif birliği olmayan kuvvet komutanlığını pasifleştirir veya aktifleştirir.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.ForcesCommands.Update)]
    public async Task<IActionResult> ChangeStatus(Guid id,
        [FromBody] ChangeForcesCommandDefStatusCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("Route ve kuvvet komutanlığı kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(command, ct));
    }

    /// <summary>Bağlı birliği olmayan kuvvet komutanlığını soft delete yapar.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.ForcesCommands.Delete)]
    public async Task<IActionResult> Delete(
        Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteForcesCommandDefCommand(id, version), ct);
        return NoContent();
    }
}

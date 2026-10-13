using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.MilitaryUnits.Commands.CreateMilitaryUnitDef;
using KLMN.Application.MilitaryUnits.Commands.UpdateMilitaryUnitDef;
using KLMN.Application.MilitaryUnits.Commands.ChangeMilitaryUnitDefStatus;
using KLMN.Application.MilitaryUnits.Commands.DeleteMilitaryUnitDef;
using KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitDefs;
using KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitDefById;
using KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitParentOptions;
using KLMN.Application.MilitaryUnits.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Birlik Tanımları: birim CRUD, hiyerarşi ve Excel endpoint'leri.</summary>
[ApiController]
[Route("api/military-units")]
public sealed class MilitaryUnitsController(ISender sender, IExcelExportService excel)
    : ControllerBase
{
    /// <summary>Birlik birimlerini arama, aktiflik ve kök filtreleriyle listeler.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.MilitaryUnits.Query)]
    public async Task<IActionResult> GetMilitaryUnits(
        [FromQuery] GetMilitaryUnitDefsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>Aranmış ve filtrelenmiş tüm kayıtları ortak XLSX servisiyle dışa aktarır.</summary>
    [HttpGet("export")]
    [HasPermission(PermissionCodes.MilitaryUnits.Query)]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    public async Task<IActionResult> ExportMilitaryUnits(
        [FromQuery] GetMilitaryUnitDefsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Excel kayıt limiti aşıldı.",
                Detail = "En fazla 5.000 birlik dışa aktarılabilir."
            });

        var items = new List<MilitaryUnitDefResponse>(first.Items);
        var totalPages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var page = 2; page <= totalPages; page++)
        {
            var batch = await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct);
            items.AddRange(batch.Items);
        }

        var columns = new ExcelColumn<MilitaryUnitDefResponse>[]
        {
            new("Birim Adı", x => x.Name, Width: 38),
            new("Birim Kodu", x => x.Code, Width: 22),
            new("Harici Kod", x => x.ExternalCode, Width: 24),
            new("Kuvvet Komutanlığı Kimliği", x => x.ForcesCommandDef, Width: 38),
            new("Geçerli Başlangıç", x => x.ValidFrom, NumberFormat: "dd.mm.yyyy hh:mm", Width: 22),
            new("Geçerli Bitiş", x => x.ValidTo, NumberFormat: "dd.mm.yyyy hh:mm", Width: 22),
            new("Üst Birim", x => x.ConnectedTopUnitName, Width: 38),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 16),
            new("Kullanıcı Sayısı", x => x.UserCount, Width: 20),
            new("Alt Birim Sayısı", x => x.ChildCount, Width: 20),
            new("Oluşturulma (UTC)", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 26)
        };

        return File(excel.Export(items, columns, "Birlikler"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Birlik_Tanimlari_{DateTime.UtcNow:yyyyMMdd_HHmm}.xlsx");
    }

    /// <summary>Yeni birim oluştururken seçilebilecek aktif üst birimleri getirir.</summary>
    [HttpGet("parent-options")]
    [HasPermission(PermissionCodes.MilitaryUnits.Query)]
    public async Task<IActionResult> GetParentOptions(CancellationToken ct) =>
        Ok(await sender.Send(new GetMilitaryUnitParentOptionsQuery(), ct));

    /// <summary>Mevcut pasif kayıtlar dahil kimlik üzerinden güncel birimi getirir.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.MilitaryUnits.Query)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetMilitaryUnitDefByIdQuery(id), ct));

    /// <summary>Yeni birlik oluşturur.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.MilitaryUnits.Create)]
    public async Task<IActionResult> Create(
        [FromBody] CreateMilitaryUnitDefCommand command, CancellationToken ct)
    {
        var created = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Güncel xmin ile birim bilgilerini ve üst birim bağlantısını değiştirir.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.MilitaryUnits.Update)]
    public async Task<IActionResult> Update(Guid id,
        [FromBody] UpdateMilitaryUnitDefCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("Route ve birlik kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(command, ct));
    }

    /// <summary>Kullanıcısı ve alt birimi olmayan birliku pasifleştirir veya aktifleştirir.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.MilitaryUnits.Update)]
    public async Task<IActionResult> ChangeStatus(Guid id,
        [FromBody] ChangeMilitaryUnitDefStatusCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("Route ve birlik kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(command, ct));
    }

    /// <summary>Kullanıcı veya alt birim bağlantısı olmayan birimi soft delete yapar.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.MilitaryUnits.Delete)]
    public async Task<IActionResult> Delete(
        Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteMilitaryUnitDefCommand(id, version), ct);
        return NoContent();
    }
}

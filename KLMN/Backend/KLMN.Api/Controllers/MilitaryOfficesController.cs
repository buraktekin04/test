using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.MilitaryOffices.Commands.CreateMilitaryOfficeDef;
using KLMN.Application.MilitaryOffices.Commands.UpdateMilitaryOfficeDef;
using KLMN.Application.MilitaryOffices.Commands.DeleteMilitaryOfficeDef;
using KLMN.Application.MilitaryOffices.Commands.ChangeStatusMilitaryOfficeDef;
using KLMN.Application.MilitaryOffices.Queries.GetMilitaryOfficeDefs;
using KLMN.Application.MilitaryOffices.Queries.GetMilitaryOfficeDefById;
using KLMN.Application.MilitaryOffices.Queries.GetMilitaryOfficeOptions;
using KLMN.Application.MilitaryOffices.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Askerlik Şubesi Tanımları için CQRS, permission ve Excel API'si.</summary>
[ApiController]
[Route("api/military-offices")]
public sealed class MilitaryOfficesController(ISender sender, IExcelExportService excel) : ControllerBase
{
    /// <summary>Arama, şehir ve aktiflik filtreleriyle sayfalı liste.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.MilitaryOffices.View, PermissionCodes.MilitaryOffices.Query,
        PermissionCodes.MilitaryOffices.Create, PermissionCodes.MilitaryOffices.Update,
        PermissionCodes.MilitaryOffices.Delete)]
    public async Task<IActionResult> List([FromQuery] GetMilitaryOfficeDefsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>Şube sorgu yetkisiyle filtrelenmiş en fazla 5000 kaydı Excel'e aktarır.</summary>
    [HttpGet("export")]
    [HasPermission(PermissionCodes.MilitaryOffices.Query)]
    public async Task<IActionResult> Export([FromQuery] GetMilitaryOfficeDefsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails {
                Status = 400, Title = "Excel aktarım sınırı aşıldı.",
                Detail = "En fazla 5.000 kayıt indirilebilir; filtreleri daraltınız."
            });
        var rows = new List<MilitaryOfficeDefResponse>(first.Items);
        var pages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var page = 2; page <= pages; page++)
        {
            var batch = await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct);
            rows.AddRange(batch.Items);
        }
        var columns = new ExcelColumn<MilitaryOfficeDefResponse>[]
        {
            new("Kod", x => x.Code, Width: 20),
            new("Askerlik Şubesi Adı", x => x.Name, Width: 42),
            new("Harici Kod", x => x.ExternalCode, Width: 20),
            new("Şehir", x => x.CityName, Width: 30),
            new("Geçerli Başlangıç", x => x.ValidFrom, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Geçerli Bitiş", x => x.ValidTo, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 15),
            new("Oluşturulma", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 24)
        };
        return File(excel.Export(rows, columns, "Askerlik Şubesi Tanımları"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "military-offices_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".xlsx");
    }

    /// <summary>Şube yetkisiyle şehir dropdown seçeneklerini sunar.</summary>
    [HttpGet("options")]
    [HasPermission(PermissionCodes.MilitaryOffices.View, PermissionCodes.MilitaryOffices.Query,
        PermissionCodes.MilitaryOffices.Create, PermissionCodes.MilitaryOffices.Update,
        PermissionCodes.MilitaryOffices.Delete)]
    public async Task<IActionResult> Options([FromQuery] Guid? selectedCityId, CancellationToken ct) =>
        Ok(await sender.Send(new GetMilitaryOfficeOptionsQuery(selectedCityId), ct));

    /// <summary>Askerlik şubesini güncel xmin sürümüyle getirir.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.MilitaryOffices.View, PermissionCodes.MilitaryOffices.Query,
        PermissionCodes.MilitaryOffices.Create, PermissionCodes.MilitaryOffices.Update,
        PermissionCodes.MilitaryOffices.Delete)]
    public async Task<IActionResult> ById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetMilitaryOfficeDefByIdQuery(id), ct));

    /// <summary>Yeni askerlik şubesi oluşturur.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.MilitaryOffices.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMilitaryOfficeDefCommand request, CancellationToken ct)
    {
        var item = await sender.Send(request, ct);
        return CreatedAtAction(nameof(ById), new { id = item.Id }, item);
    }

    /// <summary>Güncel xmin ile şube bilgilerini günceller.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.MilitaryOffices.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMilitaryOfficeDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>Şubenin aktif/pasif durumunu değiştirir.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.MilitaryOffices.Update)]
    public async Task<IActionResult> Status(Guid id, [FromBody] ChangeStatusMilitaryOfficeDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>xmin kontrolüyle soft-delete uygular.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.MilitaryOffices.Delete)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteMilitaryOfficeDefCommand(id, version), ct);
        return NoContent();
    }
}

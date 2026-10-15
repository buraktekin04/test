using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.MilitaryClasses.Commands.CreateMilitaryClassDef;
using KLMN.Application.MilitaryClasses.Commands.UpdateMilitaryClassDef;
using KLMN.Application.MilitaryClasses.Commands.ChangeStatusMilitaryClassDef;
using KLMN.Application.MilitaryClasses.Commands.DeleteMilitaryClassDef;
using KLMN.Application.MilitaryClasses.Queries.GetMilitaryClassDefs;
using KLMN.Application.MilitaryClasses.Queries.GetMilitaryClassDefById;
using KLMN.Application.MilitaryClasses.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Askerî Sınıf Tanımları API'si. Her işlem ayrı CQRS ve permission ile korunur.</summary>
[ApiController]
[Route("api/military-classes")]
public sealed class MilitaryClassesController(ISender sender, IExcelExportService excel) : ControllerBase
{
    /// <summary>Modüldeki herhangi bir yetkiyle sınıf listesi okunabilir.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.MilitaryClasses.View, PermissionCodes.MilitaryClasses.Query,
        PermissionCodes.MilitaryClasses.Create, PermissionCodes.MilitaryClasses.Update,
        PermissionCodes.MilitaryClasses.Delete)]
    public async Task<IActionResult> List([FromQuery] GetMilitaryClassDefsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>Yalnızca Query yetkisiyle filtreli sınıfları Excel'e aktarır.</summary>
    [HttpGet("export")]
    [HasPermission(PermissionCodes.MilitaryClasses.Query)]
    public async Task<IActionResult> Export([FromQuery] GetMilitaryClassDefsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails
            {
                Status = 400, Title = "Excel aktarım sınırı aşıldı.",
                Detail = "En fazla 5.000 kayıt indirilebilir; filtreleri daraltınız."
            });
        var rows = new List<MilitaryClassDefResponse>(first.Items);
        var pages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var page = 2; page <= pages; page++)
        {
            var batch = await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct);
            rows.AddRange(batch.Items);
        }

        var columns = new ExcelColumn<MilitaryClassDefResponse>[]
        {
            new("Kod", x => x.Code, Width: 22),
            new("Askerî Sınıf Adı", x => x.Name, Width: 36),
            new("Kısa Ad", x => x.ShortName, Width: 22),
            new("Harici Kod", x => x.ExternalCode, Width: 22),
            new("Active Bilgisi", x => x.Active.HasValue ? (x.Active.Value ? "Evet" : "Hayır") : "Belirtilmedi", Width: 22),
            new("Geçerli Başlangıç", x => x.ValidFrom, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Geçerli Bitiş", x => x.ValidTo, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Kayıt Durumu", x => x.IsActive ? "Aktif" : "Pasif", Width: 18),
            new("Oluşturulma", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 24)
        };
        return File(excel.Export(rows, columns, "Askerî Sınıf Tanımları"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "military-classes_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".xlsx");
    }

    /// <summary>Güncel xmin içeren sınıf detayını döndürür.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.MilitaryClasses.View, PermissionCodes.MilitaryClasses.Query,
        PermissionCodes.MilitaryClasses.Create, PermissionCodes.MilitaryClasses.Update,
        PermissionCodes.MilitaryClasses.Delete)]
    public async Task<IActionResult> ById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetMilitaryClassDefByIdQuery(id), ct));

    /// <summary>Create permission'ı bulunan kullanıcı yeni sınıf tanımlayabilir.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.MilitaryClasses.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMilitaryClassDefCommand request, CancellationToken ct)
    {
        var item = await sender.Send(request, ct);
        return CreatedAtAction(nameof(ById), new { id = item.Id }, item);
    }

    /// <summary>Mevcut sınıfı güncel xmin kullanarak düzenler.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.MilitaryClasses.Update)]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateMilitaryClassDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>IsActive durumunu değiştirir; nullable Active alanı değişmez.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.MilitaryClasses.Update)]
    public async Task<IActionResult> Status(
        Guid id, [FromBody] ChangeStatusMilitaryClassDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>Delete yetkisi ve güncel xmin ile soft-delete uygular.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.MilitaryClasses.Delete)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteMilitaryClassDefCommand(id, version), ct);
        return NoContent();
    }
}

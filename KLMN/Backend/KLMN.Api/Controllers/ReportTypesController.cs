using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.ReportTypes.Commands.CreateReportTypeDef;
using KLMN.Application.ReportTypes.Commands.UpdateReportTypeDef;
using KLMN.Application.ReportTypes.Commands.ChangeStatusReportTypeDef;
using KLMN.Application.ReportTypes.Commands.DeleteReportTypeDef;
using KLMN.Application.ReportTypes.Queries.GetReportTypeDefs;
using KLMN.Application.ReportTypes.Queries.GetReportTypeDefById;
using KLMN.Application.ReportTypes.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Rapor Türü Tanımları CRUD, Excel ve permission API'si.</summary>
[ApiController]
[Route("api/report-types")]
public sealed class ReportTypesController(ISender sender, IExcelExportService excel) : ControllerBase
{
    /// <summary>Yetkili kullanıcıya filtreli, sayfalı liste.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.ReportTypes.View, PermissionCodes.ReportTypes.Query,
        PermissionCodes.ReportTypes.Create, PermissionCodes.ReportTypes.Update,
        PermissionCodes.ReportTypes.Delete)]
    public async Task<IActionResult> List([FromQuery] GetReportTypeDefsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>En çok 5000 adet sonucu sorgu yetkisi ile Excel'e dönüştürür.</summary>
    [HttpGet("export")]
    [HasPermission(PermissionCodes.ReportTypes.Query)]
    public async Task<IActionResult> Export([FromQuery] GetReportTypeDefsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails {
                Status = 400, Title = "Excel aktarım sınırı aşıldı.",
                Detail = "En fazla 5.000 kayıt indirilebilir; filtreleri daraltınız."
            });

        var rows = new List<ReportTypeDefResponse>(first.Items);
        var pages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var page = 2; page <= pages; page++)
        {
            var batch = await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct);
            rows.AddRange(batch.Items);
        }
        var columns = new ExcelColumn<ReportTypeDefResponse>[]
        {
            new("Kod", x => x.Code, Width: 22),
            new("Rapor Türü Adı", x => x.Name, Width: 38),
            new("Geçerli Başlangıç", x => x.ValidFrom, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Geçerli Bitiş", x => x.ValidTo, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 18),
            new("Oluşturulma", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 24)
        };
        return File(excel.Export(rows, columns, "Rapor Türü Tanımları"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "report-types_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".xlsx");
    }

    /// <summary>Detayını ve eşzamanlılık sürümünü döndürür.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.ReportTypes.View, PermissionCodes.ReportTypes.Query,
        PermissionCodes.ReportTypes.Create, PermissionCodes.ReportTypes.Update,
        PermissionCodes.ReportTypes.Delete)]
    public async Task<IActionResult> ById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetReportTypeDefByIdQuery(id), ct));

    /// <summary>Rapor Türü ekler.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.ReportTypes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateReportTypeDefCommand request, CancellationToken ct)
    {
        var item = await sender.Send(request, ct);
        return CreatedAtAction(nameof(ById), new { id = item.Id }, item);
    }

    /// <summary>Güncel xmin ile rapor türü günceller.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.ReportTypes.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReportTypeDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>BaseEntity.IsActive üzerinden durum değiştirir.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.ReportTypes.Update)]
    public async Task<IActionResult> Status(Guid id, [FromBody] ChangeStatusReportTypeDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>Optimistic concurrency ile soft-delete.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.ReportTypes.Delete)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteReportTypeDefCommand(id, version), ct);
        return NoContent();
    }
}

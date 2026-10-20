using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.HospitalReportTypes.Commands.CreateHospitalReportType;
using KLMN.Application.HospitalReportTypes.Commands.UpdateHospitalReportType;
using KLMN.Application.HospitalReportTypes.Commands.ChangeStatusHospitalReportType;
using KLMN.Application.HospitalReportTypes.Commands.DeleteHospitalReportType;
using KLMN.Application.HospitalReportTypes.Queries.GetHospitalReportTypes;
using KLMN.Application.HospitalReportTypes.Queries.GetHospitalReportTypeGroups;
using KLMN.Application.HospitalReportTypes.Queries.GetHospitalReportTypeById;
using KLMN.Application.HospitalReportTypes.Responses;
using KLMN.Application.HospitalReportTypes.Queries.GetHospitalReportTypeOptions;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Sağlık Tesisi Rapor Türü Tanımları CRUD, Excel ve permission API'si.</summary>
[ApiController]
[Route("api/hospital-report-types")]
public sealed class HospitalReportTypesController(ISender sender, IExcelExportService excel) : ControllerBase
{
    /// <summary>Yetkili kullanıcıya filtreli, sayfalı liste.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.HospitalReportTypes.View, PermissionCodes.HospitalReportTypes.Query,
        PermissionCodes.HospitalReportTypes.Create, PermissionCodes.HospitalReportTypes.Update,
        PermissionCodes.HospitalReportTypes.Delete)]
    public async Task<IActionResult> List([FromQuery] GetHospitalReportTypesQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>Tesis başına bir kayıt ve alt alta rapor türleri döndürür.</summary>
    [HttpGet("groups")]
    [HasPermission(PermissionCodes.HospitalReportTypes.View, PermissionCodes.HospitalReportTypes.Query,
        PermissionCodes.HospitalReportTypes.Create, PermissionCodes.HospitalReportTypes.Update,
        PermissionCodes.HospitalReportTypes.Delete)]
    public async Task<IActionResult> Groups(
        [FromQuery] GetHospitalReportTypeGroupsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>En çok 5000 adet sonucu sorgu yetkisi ile Excel'e dönüştürür.</summary>
    [HttpGet("export")]
    [HasPermission(PermissionCodes.HospitalReportTypes.Query)]
    public async Task<IActionResult> Export([FromQuery] GetHospitalReportTypesQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails {
                Status = 400, Title = "Excel aktarım sınırı aşıldı.",
                Detail = "En fazla 5.000 kayıt indirilebilir; filtreleri daraltınız."
            });

        var rows = new List<HospitalReportTypeResponse>(first.Items);
        var pages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var page = 2; page <= pages; page++)
        {
            var batch = await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct);
            rows.AddRange(batch.Items);
        }
        var columns = new ExcelColumn<HospitalReportTypeResponse>[]
        {
            new("Sağlık Tesisi", x => x.HospitalName, Width: 38),
            new("Rapor Türü", x => x.ReportTypeName, Width: 34),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 18),
            new("Oluşturulma", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 24)
        };
        return File(excel.Export(rows, columns, "Sağlık Tesisi Rapor Türü Tanımları"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "hospital-report-types_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".xlsx");
    }

    /// <summary>Aktif sağlık tesisleri ve rapor türleri seçimi.</summary>
    [HttpGet("options")]
    [HasPermission(PermissionCodes.HospitalReportTypes.View, PermissionCodes.HospitalReportTypes.Query,
        PermissionCodes.HospitalReportTypes.Create, PermissionCodes.HospitalReportTypes.Update)]
    public async Task<IActionResult> Options([FromQuery] Guid? selectedHospitalDefId,
        [FromQuery] Guid? selectedReportTypeDefId, CancellationToken ct) =>
        Ok(await sender.Send(new GetHospitalReportTypeOptionsQuery(selectedHospitalDefId,
            selectedReportTypeDefId), ct));

    /// <summary>Detayını ve eşzamanlılık sürümünü döndürür.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.HospitalReportTypes.View, PermissionCodes.HospitalReportTypes.Query,
        PermissionCodes.HospitalReportTypes.Create, PermissionCodes.HospitalReportTypes.Update,
        PermissionCodes.HospitalReportTypes.Delete)]
    public async Task<IActionResult> ById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetHospitalReportTypeByIdQuery(id), ct));

    /// <summary>Sağlık Tesisi Rapor Türü ekler.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.HospitalReportTypes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateHospitalReportTypeCommand request, CancellationToken ct)
    {
        var item = await sender.Send(request, ct);
        return CreatedAtAction(nameof(ById), new { id = item.Id }, item);
    }

    /// <summary>Güncel xmin ile sağlık tesisi rapor türü günceller.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.HospitalReportTypes.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHospitalReportTypeCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>BaseEntity.IsActive üzerinden durum değiştirir.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.HospitalReportTypes.Update)]
    public async Task<IActionResult> Status(Guid id, [FromBody] ChangeStatusHospitalReportTypeCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>Optimistic concurrency ile soft-delete.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.HospitalReportTypes.Delete)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteHospitalReportTypeCommand(id, version), ct);
        return NoContent();
    }
}

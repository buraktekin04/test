using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.Hospitals.Commands.CreateHospitalDef;
using KLMN.Application.Hospitals.Commands.UpdateHospitalDef;
using KLMN.Application.Hospitals.Commands.ChangeStatusHospitalDef;
using KLMN.Application.Hospitals.Commands.DeleteHospitalDef;
using KLMN.Application.Hospitals.Queries.GetHospitalDefs;
using KLMN.Application.Hospitals.Queries.GetHospitalDefById;
using KLMN.Application.Hospitals.Responses;
using KLMN.Application.Hospitals.Queries.GetHospitalOptions;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Sağlık Tesisi Tanımları CRUD, Excel ve permission API'si.</summary>
[ApiController]
[Route("api/hospitals")]
public sealed class HospitalsController(ISender sender, IExcelExportService excel) : ControllerBase
{
    /// <summary>Yetkili kullanıcıya filtreli, sayfalı liste.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.Hospitals.View, PermissionCodes.Hospitals.Query,
        PermissionCodes.Hospitals.Create, PermissionCodes.Hospitals.Update,
        PermissionCodes.Hospitals.Delete)]
    public async Task<IActionResult> List([FromQuery] GetHospitalDefsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>En çok 5000 adet sonucu sorgu yetkisi ile Excel'e dönüştürür.</summary>
    [HttpGet("export")]
    [HasPermission(PermissionCodes.Hospitals.Query)]
    public async Task<IActionResult> Export([FromQuery] GetHospitalDefsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails {
                Status = 400, Title = "Excel aktarım sınırı aşıldı.",
                Detail = "En fazla 5.000 kayıt indirilebilir; filtreleri daraltınız."
            });

        var rows = new List<HospitalDefResponse>(first.Items);
        var pages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var page = 2; page <= pages; page++)
        {
            var batch = await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct);
            rows.AddRange(batch.Items);
        }
        var columns = new ExcelColumn<HospitalDefResponse>[]
        {
            new("Kod", x => x.Code, Width: 22),
            new("Sağlık Tesisi Adı", x => x.Name, Width: 38),
            new("Şehir", x => x.CityName, Width: 22),
            new("Hakem Sağlık Tesisi", x => x.RefereeHospitalName, Width: 28),
            new("Geçerli Başlangıç", x => x.ValidFrom, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Geçerli Bitiş", x => x.ValidTo, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 18),
            new("Oluşturulma", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 24)
        };
        return File(excel.Export(rows, columns, "Sağlık Tesisi Tanımları"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "hospitals_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".xlsx");
    }

    /// <summary>Şehir ve hakem sağlık tesisi dropdown verisi.</summary>
    [HttpGet("options")]
    [HasPermission(PermissionCodes.Hospitals.View, PermissionCodes.Hospitals.Query,
        PermissionCodes.Hospitals.Create, PermissionCodes.Hospitals.Update)]
    public async Task<IActionResult> Options([FromQuery] Guid? selectedCityId,
        [FromQuery] Guid? selectedRefereeHospitalId, CancellationToken ct) =>
        Ok(await sender.Send(new GetHospitalOptionsQuery(selectedCityId, selectedRefereeHospitalId), ct));

    /// <summary>Detayını ve eşzamanlılık sürümünü döndürür.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.Hospitals.View, PermissionCodes.Hospitals.Query,
        PermissionCodes.Hospitals.Create, PermissionCodes.Hospitals.Update,
        PermissionCodes.Hospitals.Delete)]
    public async Task<IActionResult> ById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetHospitalDefByIdQuery(id), ct));

    /// <summary>Sağlık Tesisi ekler.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.Hospitals.Create)]
    public async Task<IActionResult> Create([FromBody] CreateHospitalDefCommand request, CancellationToken ct)
    {
        var item = await sender.Send(request, ct);
        return CreatedAtAction(nameof(ById), new { id = item.Id }, item);
    }

    /// <summary>Güncel xmin ile sağlık tesisi günceller.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.Hospitals.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHospitalDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>BaseEntity.IsActive üzerinden durum değiştirir.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.Hospitals.Update)]
    public async Task<IActionResult> Status(Guid id, [FromBody] ChangeStatusHospitalDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>Optimistic concurrency ile soft-delete.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.Hospitals.Delete)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteHospitalDefCommand(id, version), ct);
        return NoContent();
    }
}

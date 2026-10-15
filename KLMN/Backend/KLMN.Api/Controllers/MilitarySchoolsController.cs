using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.MilitarySchools.Commands.CreateMilitarySchoolDef;
using KLMN.Application.MilitarySchools.Commands.UpdateMilitarySchoolDef;
using KLMN.Application.MilitarySchools.Commands.ChangeStatusMilitarySchoolDef;
using KLMN.Application.MilitarySchools.Commands.DeleteMilitarySchoolDef;
using KLMN.Application.MilitarySchools.Queries.GetMilitarySchoolDefs;
using KLMN.Application.MilitarySchools.Queries.GetMilitarySchoolDefById;
using KLMN.Application.MilitarySchools.Queries.GetMilitarySchoolOptions;
using KLMN.Application.MilitarySchools.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Askerî Okul Tanımları için permission kontrollü CQRS ve Excel API'si.</summary>
[ApiController]
[Route("api/military-schools")]
public sealed class MilitarySchoolsController(ISender sender, IExcelExportService excel) : ControllerBase
{
    /// <summary>Arama, şehir/kuvvet filtresi ve sayfalamalı listeyi döndürür.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.MilitarySchools.View, PermissionCodes.MilitarySchools.Query,
        PermissionCodes.MilitarySchools.Create, PermissionCodes.MilitarySchools.Update,
        PermissionCodes.MilitarySchools.Delete)]
    public async Task<IActionResult> List([FromQuery] GetMilitarySchoolDefsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>Yalnızca Query izniyle en çok 5000 filtrelenmiş satırı Excel olarak döndürür.</summary>
    [HttpGet("export")]
    [HasPermission(PermissionCodes.MilitarySchools.Query)]
    public async Task<IActionResult> Export([FromQuery] GetMilitarySchoolDefsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails
            {
                Status = 400, Title = "Excel aktarım sınırı aşıldı.",
                Detail = "En fazla 5.000 kayıt indirilebilir; filtreleri daraltınız."
            });
        var rows = new List<MilitarySchoolDefResponse>(first.Items);
        var pages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var page = 2; page <= pages; page++)
        {
            var batch = await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct);
            rows.AddRange(batch.Items);
        }

        var columns = new ExcelColumn<MilitarySchoolDefResponse>[]
        {
            new("Kod", x => x.Code, Width: 20),
            new("Askerî Okul Adı", x => x.Name, Width: 42),
            new("Şehir", x => x.CityName, Width: 30),
            new("Kuvvet Komutanlığı", x => x.ForcesCommandName, Width: 38),
            new("Geçerli Başlangıç", x => x.ValidFrom, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Geçerli Bitiş", x => x.ValidTo, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 15),
            new("Oluşturulma", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 24)
        };
        return File(excel.Export(rows, columns, "Askerî Okul Tanımları"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "military-schools_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".xlsx");
    }

    /// <summary>
    /// Okul yetkisiyle şehir ve kuvvet seçeneklerini getirir; iki modül için
    /// ek yönetim yetkisi vermeden formun kullanılmasını sağlar.
    /// </summary>
    [HttpGet("options")]
    [HasPermission(PermissionCodes.MilitarySchools.View, PermissionCodes.MilitarySchools.Query,
        PermissionCodes.MilitarySchools.Create, PermissionCodes.MilitarySchools.Update,
        PermissionCodes.MilitarySchools.Delete)]
    public async Task<IActionResult> Options(
        [FromQuery] Guid? selectedCityId,
        [FromQuery] Guid? selectedForcesCommandDefId,
        CancellationToken ct) =>
        Ok(await sender.Send(
            new GetMilitarySchoolOptionsQuery(selectedCityId, selectedForcesCommandDefId), ct));

    /// <summary>Düzenlemede kullanılacak güncel sürüm dahil okul detayını getirir.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.MilitarySchools.View, PermissionCodes.MilitarySchools.Query,
        PermissionCodes.MilitarySchools.Create, PermissionCodes.MilitarySchools.Update,
        PermissionCodes.MilitarySchools.Delete)]
    public async Task<IActionResult> ById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetMilitarySchoolDefByIdQuery(id), ct));

    /// <summary>Create eylem izni ile askerî okul oluşturur.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.MilitarySchools.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMilitarySchoolDefCommand request, CancellationToken ct)
    {
        var school = await sender.Send(request, ct);
        return CreatedAtAction(nameof(ById), new { id = school.Id }, school);
    }

    /// <summary>Güncel xmin ile okul bilgilerinin değiştirilmesini sağlar.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.MilitarySchools.Update)]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateMilitarySchoolDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>Aktivasyon sırasında bağlı şehir ve kuvvetin aktifliğini kontrol eder.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.MilitarySchools.Update)]
    public async Task<IActionResult> Status(
        Guid id, [FromBody] ChangeStatusMilitarySchoolDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>Delete eylem izni ve güncel xmin ile soft-delete yapar.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.MilitarySchools.Delete)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteMilitarySchoolDefCommand(id, version), ct);
        return NoContent();
    }
}

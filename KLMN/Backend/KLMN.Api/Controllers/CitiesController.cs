using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.Cities.Commands.CreateCityDef;
using KLMN.Application.Cities.Commands.UpdateCityDef;
using KLMN.Application.Cities.Commands.ChangeStatusCityDef;
using KLMN.Application.Cities.Commands.DeleteCityDef;
using KLMN.Application.Cities.Queries.GetCityDefs;
using KLMN.Application.Cities.Queries.GetCityDefById;
using KLMN.Application.Cities.Responses;
using KLMN.Application.Cities.Queries.GetCityCountryOptions;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

[ApiController]
[Route("api/cities")]
public sealed class CitiesController(ISender sender, IExcelExportService excel) : ControllerBase
{
    [HttpGet]
    [HasPermission(PermissionCodes.Cities.View, PermissionCodes.Cities.Query,
        PermissionCodes.Cities.Create, PermissionCodes.Cities.Update, PermissionCodes.Cities.Delete)]
    public async Task<IActionResult> List([FromQuery] GetCityDefsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    [HttpGet("export")]
    [HasPermission(PermissionCodes.Cities.Query)]
    public async Task<IActionResult> Export([FromQuery] GetCityDefsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber=1, PageSize=pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails { Status=400, Title="Excel aktarım sınırı aşıldı.",
                Detail="En fazla 5.000 kayıt indirilebilir; filtreleri daraltınız." });
        var rows = new List<CityDefResponse>(first.Items);
        var pages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var p = 2; p <= pages; p++)
        {
            var batch = await sender.Send(query with { PageNumber=p, PageSize=pageSize }, ct);
            rows.AddRange(batch.Items);
        }
        var columns = new ExcelColumn<CityDefResponse>[]
        {
            new("Kod", x => x.Code, Width: 22),
            new("Şehir Adı", x => x.Name, Width: 36),
            new("Harici Kod", x => x.ExternalCode, Width: 24),
            new("Ülke", x => x.CountryName, Width: 32),
            new("MERNİS Kodu", x => x.MernisCode, Width: 20),
            new("Geçerli Başlangıç", x => x.ValidFrom, NumberFormat: "dd.MM.yyyy HH:mm", Width: 26),
            new("Geçerli Bitiş", x => x.ValidTo, NumberFormat: "dd.MM.yyyy HH:mm", Width: 26),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 16),
            new("Oluşturulma", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25)
        };
        return File(excel.Export(rows, columns, "Şehir Tanımları"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "cities_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".xlsx");
    }

    [HttpGet("country-options")]
    [HasPermission(PermissionCodes.Cities.View, PermissionCodes.Cities.Query,
        PermissionCodes.Cities.Create, PermissionCodes.Cities.Update, PermissionCodes.Cities.Delete)]
    public async Task<IActionResult> GetCountryOptions(
        [FromQuery] Guid? selectedCountryId, CancellationToken ct) =>
        Ok(await sender.Send(new GetCityCountryOptionsQuery(selectedCountryId), ct));
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.Cities.View, PermissionCodes.Cities.Query,
        PermissionCodes.Cities.Create, PermissionCodes.Cities.Update, PermissionCodes.Cities.Delete)]
    public async Task<IActionResult> ById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetCityDefByIdQuery(id), ct));

    [HttpPost]
    [HasPermission(PermissionCodes.Cities.Create)]
    public async Task<IActionResult> Create([FromBody] CreateCityDefCommand request, CancellationToken ct)
    {
        var item = await sender.Send(request, ct);
        return CreatedAtAction(nameof(ById), new { id=item.Id }, item);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.Cities.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCityDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.Cities.Update)]
    public async Task<IActionResult> Status(Guid id, [FromBody] ChangeStatusCityDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.Cities.Delete)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteCityDefCommand(id, version), ct);
        return NoContent();
    }
}

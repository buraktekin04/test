using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.Countries.Commands.CreateCountryDef;
using KLMN.Application.Countries.Commands.UpdateCountryDef;
using KLMN.Application.Countries.Commands.ChangeStatusCountryDef;
using KLMN.Application.Countries.Commands.DeleteCountryDef;
using KLMN.Application.Countries.Queries.GetCountryDefs;
using KLMN.Application.Countries.Queries.GetCountryDefById;
using KLMN.Application.Countries.Responses;

using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

[ApiController]
[Route("api/countries")]
public sealed class CountriesController(ISender sender, IExcelExportService excel) : ControllerBase
{
    [HttpGet]
    [HasPermission(PermissionCodes.Countries.View, PermissionCodes.Countries.Query,
        PermissionCodes.Countries.Create, PermissionCodes.Countries.Update, PermissionCodes.Countries.Delete)]
    public async Task<IActionResult> List([FromQuery] GetCountryDefsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    [HttpGet("export")]
    [HasPermission(PermissionCodes.Countries.Query)]
    public async Task<IActionResult> Export([FromQuery] GetCountryDefsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber=1, PageSize=pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails { Status=400, Title="Excel aktarım sınırı aşıldı.",
                Detail="En fazla 5.000 kayıt indirilebilir; filtreleri daraltınız." });
        var rows = new List<CountryDefResponse>(first.Items);
        var pages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var p = 2; p <= pages; p++)
        {
            var batch = await sender.Send(query with { PageNumber=p, PageSize=pageSize }, ct);
            rows.AddRange(batch.Items);
        }
        var columns = new ExcelColumn<CountryDefResponse>[]
        {
            new("Kod", x => x.Code, Width: 22),
            new("Ülke Adı", x => x.Name, Width: 36),
            new("Harici Kod", x => x.ExternalCode, Width: 24),
            new("İlçe Zorunlu", x => x.IsTownRequired.HasValue ? (x.IsTownRequired.Value ? "Evet" : "Hayır") : "Belirsiz", Width: 20),
            new("Şehir Sayısı", x => x.CityCount, Width: 20),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 16),
            new("Oluşturulma", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25)
        };
        return File(excel.Export(rows, columns, "Ülke Tanımları"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "countries_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".xlsx");
    }

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.Countries.View, PermissionCodes.Countries.Query,
        PermissionCodes.Countries.Create, PermissionCodes.Countries.Update, PermissionCodes.Countries.Delete)]
    public async Task<IActionResult> ById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetCountryDefByIdQuery(id), ct));

    [HttpPost]
    [HasPermission(PermissionCodes.Countries.Create)]
    public async Task<IActionResult> Create([FromBody] CreateCountryDefCommand request, CancellationToken ct)
    {
        var item = await sender.Send(request, ct);
        return CreatedAtAction(nameof(ById), new { id=item.Id }, item);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.Countries.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCountryDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.Countries.Update)]
    public async Task<IActionResult> Status(Guid id, [FromBody] ChangeStatusCountryDefCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.Countries.Delete)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteCountryDefCommand(id, version), ct);
        return NoContent();
    }
}

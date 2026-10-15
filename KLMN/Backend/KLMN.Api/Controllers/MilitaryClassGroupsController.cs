using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.MilitaryClassGroups.Commands.CreateMilitaryClassGroup;
using KLMN.Application.MilitaryClassGroups.Commands.UpdateMilitaryClassGroup;
using KLMN.Application.MilitaryClassGroups.Commands.ChangeStatusMilitaryClassGroup;
using KLMN.Application.MilitaryClassGroups.Commands.DeleteMilitaryClassGroup;
using KLMN.Application.MilitaryClassGroups.Queries.GetMilitaryClassGroups;
using KLMN.Application.MilitaryClassGroups.Queries.GetMilitaryClassGroupById;
using KLMN.Application.MilitaryClassGroups.Queries.GetMilitaryClassGroupOptions;
using KLMN.Application.MilitaryClassGroups.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Askerî Sınıf Grubu Tanımları için CQRS, eylem yetkileri, Excel ve dropdown API'si.</summary>
[ApiController]
[Route("api/military-class-groups")]
public sealed class MilitaryClassGroupsController(ISender sender, IExcelExportService excel) : ControllerBase
{
    /// <summary>Herhangi bir modül izniyle sayfalı liste okunabilir.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.MilitaryClassGroups.View, PermissionCodes.MilitaryClassGroups.Query, PermissionCodes.MilitaryClassGroups.Create, PermissionCodes.MilitaryClassGroups.Update, PermissionCodes.MilitaryClassGroups.Delete)]
    public async Task<IActionResult> List([FromQuery] GetMilitaryClassGroupsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>Yalnızca Query izniyle filtreli en çok 5000 kayıt Excel'e aktarılır.</summary>
    [HttpGet("export")]
    [HasPermission(PermissionCodes.MilitaryClassGroups.Query)]
    public async Task<IActionResult> Export([FromQuery] GetMilitaryClassGroupsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails { Status = 400,
                Title = "Excel sınırı aşıldı.", Detail = "En fazla 5000 kayıt dışa aktarılabilir." });
        var rows = new List<MilitaryClassGroupResponse>(first.Items);
        var pages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (int page = 2; page <= pages; page++)
            rows.AddRange((await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct)).Items);
        var columns = new ExcelColumn<MilitaryClassGroupResponse>[] {
            new("Kod", x => x.Code, Width: 18),
            new("Ad", x => x.Name, Width: 35),
            new("Kısa Ad", x => x.ShortName, Width: 20),
            new("Kuvvet Komutanlığı", x => x.ForcesCommandName, Width: 35),
            new("Başlangıç", x => x.ValidFrom, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Bitiş", x => x.ValidTo, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 14),
            new("Oluşturulma", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 23)
        };
        return File(excel.Export(rows, columns, "Askerî Sınıf Grubu Tanımları"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "military-class-groups_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".xlsx");
    }

    /// <summary>Modül izni, üst tanımların yönetim yetkisi olmadan dropdown kullanılmasını sağlar.</summary>
    [HttpGet("options")]
    [HasPermission(PermissionCodes.MilitaryClassGroups.View, PermissionCodes.MilitaryClassGroups.Query, PermissionCodes.MilitaryClassGroups.Create, PermissionCodes.MilitaryClassGroups.Update, PermissionCodes.MilitaryClassGroups.Delete)]
    public async Task<IActionResult> Options([FromQuery] Guid? selectedForceId, CancellationToken ct) =>
        Ok(await sender.Send(new GetMilitaryClassGroupOptionsQuery(selectedForceId), ct));

    /// <summary>Güncel xmin sürümlü kayıt detayıdır.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.MilitaryClassGroups.View, PermissionCodes.MilitaryClassGroups.Query, PermissionCodes.MilitaryClassGroups.Create, PermissionCodes.MilitaryClassGroups.Update, PermissionCodes.MilitaryClassGroups.Delete)]
    public async Task<IActionResult> ById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetMilitaryClassGroupByIdQuery(id), ct));

    /// <summary>Yeni kaydı Create izniyle oluşturur.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.MilitaryClassGroups.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMilitaryClassGroupCommand request, CancellationToken ct)
    {
        var result = await sender.Send(request, ct);
        return CreatedAtAction(nameof(ById), new { id = result.Id }, result);
    }

    /// <summary>xmin ile Update izninde düzenler.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.MilitaryClassGroups.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMilitaryClassGroupCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimliği uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>İlgili aktif referans kısıtlarıyla IsActive durumunu değiştirir.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.MilitaryClassGroups.Update)]
    public async Task<IActionResult> Status(Guid id, [FromBody] ChangeStatusMilitaryClassGroupCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimliği uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>Delete izniyle soft-delete ve xmin kontrolü uygular.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.MilitaryClassGroups.Delete)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteMilitaryClassGroupCommand(id, version), ct);
        return NoContent();
    }
}

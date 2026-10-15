using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.MilitaryClassGroupRelations.Commands.CreateMilitaryClassGroupRelation;
using KLMN.Application.MilitaryClassGroupRelations.Commands.UpdateMilitaryClassGroupRelation;
using KLMN.Application.MilitaryClassGroupRelations.Commands.ChangeStatusMilitaryClassGroupRelation;
using KLMN.Application.MilitaryClassGroupRelations.Commands.DeleteMilitaryClassGroupRelation;
using KLMN.Application.MilitaryClassGroupRelations.Queries.GetMilitaryClassGroupRelations;
using KLMN.Application.MilitaryClassGroupRelations.Queries.GetMilitaryClassGroupRelationById;
using KLMN.Application.MilitaryClassGroupRelations.Queries.GetMilitaryClassGroupRelationOptions;
using KLMN.Application.MilitaryClassGroupRelations.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Askerî Sınıf–Grup Eşleştirmeleri için CQRS, eylem yetkileri, Excel ve dropdown API'si.</summary>
[ApiController]
[Route("api/military-class-group-relations")]
public sealed class MilitaryClassGroupRelationsController(ISender sender, IExcelExportService excel) : ControllerBase
{
    /// <summary>Herhangi bir modül izniyle sayfalı liste okunabilir.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.MilitaryClassGroupRelations.View, PermissionCodes.MilitaryClassGroupRelations.Query, PermissionCodes.MilitaryClassGroupRelations.Create, PermissionCodes.MilitaryClassGroupRelations.Update, PermissionCodes.MilitaryClassGroupRelations.Delete)]
    public async Task<IActionResult> List([FromQuery] GetMilitaryClassGroupRelationsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>Yalnızca Query izniyle filtreli en çok 5000 kayıt Excel'e aktarılır.</summary>
    [HttpGet("export")]
    [HasPermission(PermissionCodes.MilitaryClassGroupRelations.Query)]
    public async Task<IActionResult> Export([FromQuery] GetMilitaryClassGroupRelationsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails { Status = 400,
                Title = "Excel sınırı aşıldı.", Detail = "En fazla 5000 kayıt dışa aktarılabilir." });
        var rows = new List<MilitaryClassGroupRelationResponse>(first.Items);
        var pages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (int page = 2; page <= pages; page++)
            rows.AddRange((await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct)).Items);
        var columns = new ExcelColumn<MilitaryClassGroupRelationResponse>[] {

            new("Kuvvet Komutanlığı", x => x.ForcesCommandName, Width: 35),
            new("Askerî Sınıf", x => x.MilitaryClassName, Width: 35),
            new("Askerî Sınıf Grubu", x => x.MilitaryClassGroupName, Width: 35),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 14),
            new("Oluşturulma", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 23)
        };
        return File(excel.Export(rows, columns, "Askerî Sınıf–Grup Eşleştirmeleri"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "military-class-group-relations_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".xlsx");
    }

    /// <summary>Modül izni, üst tanımların yönetim yetkisi olmadan dropdown kullanılmasını sağlar.</summary>
    [HttpGet("options")]
    [HasPermission(PermissionCodes.MilitaryClassGroupRelations.View, PermissionCodes.MilitaryClassGroupRelations.Query, PermissionCodes.MilitaryClassGroupRelations.Create, PermissionCodes.MilitaryClassGroupRelations.Update, PermissionCodes.MilitaryClassGroupRelations.Delete)]
    public async Task<IActionResult> Options([FromQuery] Guid? forcesCommandId, [FromQuery] Guid? selectedForceId,
        [FromQuery] Guid? selectedClassId, [FromQuery] Guid? selectedGroupId, CancellationToken ct) =>
        Ok(await sender.Send(new GetMilitaryClassGroupRelationOptionsQuery(forcesCommandId, selectedForceId,
            selectedClassId, selectedGroupId), ct));

    /// <summary>Güncel xmin sürümlü kayıt detayıdır.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.MilitaryClassGroupRelations.View, PermissionCodes.MilitaryClassGroupRelations.Query, PermissionCodes.MilitaryClassGroupRelations.Create, PermissionCodes.MilitaryClassGroupRelations.Update, PermissionCodes.MilitaryClassGroupRelations.Delete)]
    public async Task<IActionResult> ById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetMilitaryClassGroupRelationByIdQuery(id), ct));

    /// <summary>Yeni kaydı Create izniyle oluşturur.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.MilitaryClassGroupRelations.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMilitaryClassGroupRelationCommand request, CancellationToken ct)
    {
        var result = await sender.Send(request, ct);
        return CreatedAtAction(nameof(ById), new { id = result.Id }, result);
    }

    /// <summary>xmin ile Update izninde düzenler.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.MilitaryClassGroupRelations.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMilitaryClassGroupRelationCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimliği uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>İlgili aktif referans kısıtlarıyla IsActive durumunu değiştirir.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.MilitaryClassGroupRelations.Update)]
    public async Task<IActionResult> Status(Guid id, [FromBody] ChangeStatusMilitaryClassGroupRelationCommand request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("Kayıt kimliği uyuşmuyor.");
        return Ok(await sender.Send(request, ct));
    }

    /// <summary>Delete izniyle soft-delete ve xmin kontrolü uygular.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.MilitaryClassGroupRelations.Delete)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteMilitaryClassGroupRelationCommand(id, version), ct);
        return NoContent();
    }
}

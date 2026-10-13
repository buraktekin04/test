using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.Organizations.Commands.CreateOrganizationUnit;
using KLMN.Application.Organizations.Commands.UpdateOrganizationUnit;
using KLMN.Application.Organizations.Commands.ChangeOrganizationUnitStatus;
using KLMN.Application.Organizations.Commands.DeleteOrganizationUnit;
using KLMN.Application.Organizations.Queries.GetOrganizationUnits;
using KLMN.Application.Organizations.Queries.GetOrganizationUnitById;
using KLMN.Application.Organizations.Queries.GetOrganizationParentOptions;
using KLMN.Application.Organizations.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Organizasyon Tanımları: birim CRUD, hiyerarşi ve Excel endpoint'leri.</summary>
[ApiController]
[Route("api/organizations")]
public sealed class OrganizationsController(ISender sender, IExcelExportService excel)
    : ControllerBase
{
    /// <summary>Organizasyon birimlerini arama, aktiflik ve kök filtreleriyle listeler.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.Organizations.Query)]
    public async Task<IActionResult> GetOrganizations(
        [FromQuery] GetOrganizationUnitsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>Aranmış ve filtrelenmiş tüm kayıtları ortak XLSX servisiyle dışa aktarır.</summary>
    [HttpGet("export")]
    [HasPermission(PermissionCodes.Organizations.Query)]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    public async Task<IActionResult> ExportOrganizations(
        [FromQuery] GetOrganizationUnitsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Excel kayıt limiti aşıldı.",
                Detail = "En fazla 5.000 organizasyon birimi dışa aktarılabilir."
            });

        var items = new List<OrganizationUnitResponse>(first.Items);
        var totalPages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var page = 2; page <= totalPages; page++)
        {
            var batch = await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct);
            items.AddRange(batch.Items);
        }

        var columns = new ExcelColumn<OrganizationUnitResponse>[]
        {
            new("Birim Adı", x => x.Name, Width: 38),
            new("Birim Kodu", x => x.Code, Width: 22),
            new("Üst Birim", x => x.ParentOrganizationUnitName, Width: 38),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 16),
            new("Kullanıcı Sayısı", x => x.UserCount, Width: 20),
            new("Alt Birim Sayısı", x => x.ChildCount, Width: 20),
            new("Oluşturulma (UTC)", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 26)
        };

        return File(excel.Export(items, columns, "Organizasyonlar"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Organizasyon_Tanimlari_{DateTime.UtcNow:yyyyMMdd_HHmm}.xlsx");
    }

    /// <summary>Yeni birim oluştururken seçilebilecek aktif üst birimleri getirir.</summary>
    [HttpGet("parent-options")]
    [HasPermission(PermissionCodes.Organizations.Query)]
    public async Task<IActionResult> GetParentOptions(CancellationToken ct) =>
        Ok(await sender.Send(new GetOrganizationParentOptionsQuery(), ct));

    /// <summary>Mevcut pasif kayıtlar dahil kimlik üzerinden güncel birimi getirir.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.Organizations.Query)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetOrganizationUnitByIdQuery(id), ct));

    /// <summary>Yeni organizasyon birimi oluşturur.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.Organizations.Create)]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrganizationUnitCommand command, CancellationToken ct)
    {
        var created = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Güncel xmin ile birim bilgilerini ve üst birim bağlantısını değiştirir.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.Organizations.Update)]
    public async Task<IActionResult> Update(Guid id,
        [FromBody] UpdateOrganizationUnitCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("Route ve organizasyon kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(command, ct));
    }

    /// <summary>Kullanıcısı ve alt birimi olmayan organizasyonu pasifleştirir veya aktifleştirir.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.Organizations.Update)]
    public async Task<IActionResult> ChangeStatus(Guid id,
        [FromBody] ChangeOrganizationUnitStatusCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("Route ve organizasyon kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(command, ct));
    }

    /// <summary>Kullanıcı veya alt birim bağlantısı olmayan birimi soft delete yapar.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.Organizations.Delete)]
    public async Task<IActionResult> Delete(
        Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteOrganizationUnitCommand(id, version), ct);
        return NoContent();
    }
}

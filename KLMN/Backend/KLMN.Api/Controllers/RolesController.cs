using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.Roles.Commands.CreateRole;
using KLMN.Application.Roles.Commands.UpdateRole;
using KLMN.Application.Roles.Commands.ChangeRoleStatus;
using KLMN.Application.Roles.Commands.DeleteRole;
using KLMN.Application.Roles.Commands.UpdateRolePermissions;
using KLMN.Application.Roles.Queries.GetRoles;
using KLMN.Application.Roles.Queries.GetRoleById;
using KLMN.Application.Roles.Queries.GetRolePermissionOptions;
using KLMN.Application.Roles.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Rol Tanımları için CRUD, yetki yönetimi ve Excel dışa aktarma endpoint'leri.</summary>
[ApiController]
[Route("api/roles")]
public sealed class RolesController(ISender sender, IExcelExportService excelExportService)
    : ControllerBase
{
    /// <summary>Rol listesini yetkili, filtreli ve sayfalı olarak sunar.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.Roles.Query)]
    public async Task<IActionResult> GetRoles(
        [FromQuery] GetRolesQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>Rol listesinin tüm filtrelenmiş kayıtlarını mevcut ortak Excel servisine gönderir.</summary>
    [HttpGet("export")]
    [HasPermission(PermissionCodes.Roles.Query)]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    public async Task<IActionResult> ExportRoles(
        [FromQuery] GetRolesQuery query, CancellationToken ct)
    {
        const int pageSize = 100;
        const int maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);

        if (first.TotalCount > maxRows)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Excel kayıt limiti aşıldı.",
                Detail = "Rol dışa aktarımı için en fazla 5.000 kayıt seçilebilir."
            });
        }

        var items = new List<RoleResponse>(first.Items);
        var pageCount = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var page = 2; page <= pageCount; page++)
        {
            var batch = await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct);
            items.AddRange(batch.Items);
        }

        var columns = new ExcelColumn<RoleResponse>[]
        {
            new("Rol Adı", x => x.Name, Width: 28),
            new("Rol Kodu", x => x.Code, Width: 28),
            new("Açıklama", x => x.Description, Width: 46),
            new("Rol Tipi", x => x.IsSystemRole ? "Sistem" : "Özel", Width: 16),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 16),
            new("Kullanıcı Sayısı", x => x.UserCount, Width: 18),
            new("Yetki Sayısı", x => x.PermissionCodes.Count, Width: 16),
            new("Oluşturulma (UTC)", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25)
        };

        var output = excelExportService.Export(items, columns, "Roller");
        return File(output,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Rol_Tanimlari_{DateTime.UtcNow:yyyyMMdd_HHmm}.xlsx");
    }

    /// <summary>Rol izin paneline mevcut izin kataloğunu getirir.</summary>
    [HttpGet("permission-options")]
    [HasPermission(PermissionCodes.Roles.ManagePermissions)]
    public async Task<IActionResult> GetPermissionOptions(CancellationToken ct) =>
        Ok(await sender.Send(new GetRolePermissionOptionsQuery(), ct));

    /// <summary>Role ait güncel alanları, kullanıcı sayısını ve atanmış permission'ları getirir.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.Roles.Query)]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoleById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetRoleByIdQuery(id), ct));

    /// <summary>Yeni rol oluşturur ve güncel xmin versiyonunu döndürür.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.Roles.Create)]
    [ProducesResponseType(typeof(RoleMutationResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateRole(
        [FromBody] CreateRoleCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetRoleById), new { id = result.Id }, result);
    }

    /// <summary>İstemcinin beklediği role optimistic concurrency ile güncelleme uygular.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.Roles.Update)]
    public async Task<IActionResult> UpdateRole(
        Guid id, [FromBody] UpdateRoleCommand command, CancellationToken ct)
    {
        if (command.Id != id) return BadRequest("Route ve gönderilen rol kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(command, ct));
    }

    /// <summary>Özel rolün aktif/pasif durumunu değiştirir.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.Roles.Update)]
    public async Task<IActionResult> ChangeStatus(
        Guid id, [FromBody] ChangeRoleStatusCommand command, CancellationToken ct)
    {
        if (command.Id != id) return BadRequest("Route ve gönderilen rol kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(command, ct));
    }

    /// <summary>İzinleri tek transaction içinde günceller ve rol xmin değerini yükseltir.</summary>
    [HttpPut("{id:guid}/permissions")]
    [HasPermission(PermissionCodes.Roles.ManagePermissions)]
    public async Task<IActionResult> UpdatePermissions(
        Guid id, [FromBody] UpdateRolePermissionsCommand command, CancellationToken ct)
    {
        if (command.Id != id) return BadRequest("Route ve gönderilen rol kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(command, ct));
    }

    /// <summary>Kullanıcı ataması olmayan özel rolü soft delete eder.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.Roles.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteRole(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteRoleCommand(id, version), ct);
        return NoContent();
    }
}

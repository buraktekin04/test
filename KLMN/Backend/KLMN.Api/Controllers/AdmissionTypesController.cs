using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Application.AdmissionTypes.Commands.CreateHcAdmissionTypeDef;
using KLMN.Application.AdmissionTypes.Commands.UpdateHcAdmissionTypeDef;
using KLMN.Application.AdmissionTypes.Commands.ChangeStatusHcAdmissionTypeDef;
using KLMN.Application.AdmissionTypes.Commands.DeleteHcAdmissionTypeDef;
using KLMN.Application.AdmissionTypes.Queries.GetHcAdmissionTypeDefs;
using KLMN.Application.AdmissionTypes.Queries.GetHcAdmissionTypeDefById;
using KLMN.Application.AdmissionTypes.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Başvuru Grubu Tanımları: CQRS, permission ve Excel uç noktaları.</summary>
[ApiController]
[Route("api/admission-types")]
public sealed class AdmissionTypesController(ISender sender, IExcelExportService excel) : ControllerBase
{
    /// <summary>Yetki ve standart sayfalı sorguyla listeyi getirir.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.AdmissionTypes.View, PermissionCodes.AdmissionTypes.Query,
        PermissionCodes.AdmissionTypes.Create, PermissionCodes.AdmissionTypes.Update,
        PermissionCodes.AdmissionTypes.Delete)]
    public async Task<IActionResult> List([FromQuery] GetHcAdmissionTypeDefsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    /// <summary>Query yetkisiyle en fazla 5000 kaydı tüm zorunluluk alanlarıyla dışa aktarır.</summary>
    [HttpGet("export")]
    [HasPermission(PermissionCodes.AdmissionTypes.Query)]
    public async Task<IActionResult> Export([FromQuery] GetHcAdmissionTypeDefsQuery query, CancellationToken ct)
    {
        const int pageSize = 100, maxRows = 5000;
        var first = await sender.Send(query with { PageNumber = 1, PageSize = pageSize }, ct);
        if (first.TotalCount > maxRows)
            return BadRequest(new ProblemDetails
            {
                Status = 400, Title = "Excel aktarım sınırı aşıldı.",
                Detail = "En fazla 5.000 kayıt indirilebilir; filtreleri daraltınız."
            });
        var rows = new List<HcAdmissionTypeDefResponse>(first.Items);
        var pages = (int)Math.Ceiling(first.TotalCount / (double)pageSize);
        for (var page = 2; page <= pages; page++)
        {
            var batch = await sender.Send(query with { PageNumber = page, PageSize = pageSize }, ct);
            rows.AddRange(batch.Items);
        }
        var columns = new ExcelColumn<HcAdmissionTypeDefResponse>[]
        {
            new("Kod", x => x.Code, Width: 16),
            new("Başvuru Grubu Adı", x => x.Name, Width: 38),
            new("Kuvvet Komutanlığı", x => x.ForcesCommandRequired ? "Zorunlu" : "Değil", Width: 26),
            new("Rütbe", x => x.RankRequired ? "Zorunlu" : "Değil", Width: 26),
            new("Askerî Sınıf", x => x.MilitaryClassRequired ? "Zorunlu" : "Değil", Width: 26),
            new("Birlik", x => x.MilitaryUnitRequired ? "Zorunlu" : "Değil", Width: 26),
            new("Gönderen Makam", x => x.SenderChairRequired ? "Zorunlu" : "Değil", Width: 26),
            new("Askerlik Şubesi", x => x.MilitaryOfficeRequired ? "Zorunlu" : "Değil", Width: 26),
            new("Sicil Numarası", x => x.RegistryNumberRequired ? "Zorunlu" : "Değil", Width: 26),
            new("Askerî Okul", x => x.MilitarySchoolRequired ? "Zorunlu" : "Değil", Width: 26),
            new("Son Adaylık Tarihi", x => x.LastNominationDateRequired ? "Zorunlu" : "Değil", Width: 26),
            new("Boy ve Kilo", x => x.HeightWeightRequired ? "Zorunlu" : "Değil", Width: 26),
            new("Kaçıncı İşlem Gerekir", x => x.NthAdmissionRequired ? "Zorunlu" : "Değil", Width: 26),
            new("Sevk Belgesi Tarihi", x => x.DispatchDocumentDateRequired ? "Zorunlu" : "Değil", Width: 26),
            new("Sevk Belgesi Numarası", x => x.DispatchDocumentNoRequired ? "Zorunlu" : "Değil", Width: 26),
            new("PERTEM Onayı", x => x.PertemApprovalRequired ? "Zorunlu" : "Değil", Width: 26),
            new("Geçerli Başlangıç", x => x.ValidFrom, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Geçerli Bitiş", x => x.ValidTo, NumberFormat: "dd.MM.yyyy HH:mm", Width: 25),
            new("Durum", x => x.IsActive ? "Aktif" : "Pasif", Width: 18),
            new("Oluşturulma", x => x.CreatedDate, NumberFormat: "dd.MM.yyyy HH:mm", Width: 24)
        };
        return File(excel.Export(rows, columns, "Başvuru Grubu Tanımları"),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "admission-types_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".xlsx");
    }

    /// <summary>Düzenlenecek kaydın bütün boolean alanlarını ve xmin sürümünü döner.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.AdmissionTypes.View, PermissionCodes.AdmissionTypes.Query,
        PermissionCodes.AdmissionTypes.Create, PermissionCodes.AdmissionTypes.Update,
        PermissionCodes.AdmissionTypes.Delete)]
    public async Task<IActionResult> ById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetHcAdmissionTypeDefByIdQuery(id), ct));

    /// <summary>Yeni başvuru grubu oluşturur.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.AdmissionTypes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateHcAdmissionTypeDefCommand command, CancellationToken ct)
    {
        var item = await sender.Send(command, ct);
        return CreatedAtAction(nameof(ById), new { id = item.Id }, item);
    }

    /// <summary>Mevcut başvuru grubu ayarlarını xmin ile günceller.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.AdmissionTypes.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHcAdmissionTypeDefCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(command, ct));
    }

    /// <summary>BaseEntity.IsActive'i status API üzerinden yönetir.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.AdmissionTypes.Update)]
    public async Task<IActionResult> Status(Guid id, [FromBody] ChangeStatusHcAdmissionTypeDefCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("Kayıt kimlikleri uyuşmuyor.");
        return Ok(await sender.Send(command, ct));
    }

    /// <summary>xmin sürümüyle soft-delete uygular.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.AdmissionTypes.Delete)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] uint version, CancellationToken ct)
    {
        await sender.Send(new DeleteHcAdmissionTypeDefCommand(id, version), ct);
        return NoContent();
    }
}

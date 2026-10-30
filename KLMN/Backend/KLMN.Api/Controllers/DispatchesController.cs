using KLMN.Api.Authorization;
using KLMN.Application.Dispatches.Commands.CancelDispatch;
using KLMN.Application.Dispatches.Commands.SetDispatchPhoto;
using KLMN.Application.Dispatches.Commands.DeleteDispatchPhoto;
using KLMN.Application.Dispatches.Queries.GetDispatchPhoto;
using KLMN.Application.Dispatches.Commands.CreateDispatch;
using KLMN.Application.Dispatches.Commands.UpdateDispatch;
using KLMN.Application.Dispatches.Commands.UploadDispatchFile;
using KLMN.Application.Dispatches.Queries.GetDispatchById;
using KLMN.Application.Dispatches.Queries.GetDispatchAdmissionRules;
using KLMN.Application.Dispatches.Queries.GetDispatches;
using KLMN.Application.Dispatches.Queries.GetDispatchStatuses;
using KLMN.Application.Dispatches.Queries.GetDispatchOptions;
using KLMN.Application.Dispatches.Queries.GetDispatchFile;
using KLMN.Application.Dispatches.Queries.GetDispatchFiles;
using KLMN.Application.Dispatches.Queries.LookupDispatchPerson;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Sevk Giriş, yerel kişi eşleştirme, sevk takibi ve belge yönetimi API'si.</summary>
[ApiController]
[Route("api/dispatches")]
public sealed class DispatchesController(ISender sender):ControllerBase
{
    /// <summary>Aktif sevk durumları veritabanı sözlüğünden okunur; tanım ekranı yoktur.</summary>
    [HttpGet("statuses")]
    [HasPermission(PermissionCodes.Dispatches.View,PermissionCodes.Dispatches.Query,PermissionCodes.Dispatches.Create,PermissionCodes.Dispatches.Update)]
    public async Task<IActionResult> Statuses(CancellationToken ct)
    {
        return Ok(await sender.Send(new GetDispatchStatusesQuery(),ct));
    }

    [HttpGet]
    [HasPermission(PermissionCodes.Dispatches.View,PermissionCodes.Dispatches.Query)]
    public async Task<IActionResult> List([FromQuery] GetDispatchesQuery query,CancellationToken ct)
    {
        return Ok(await sender.Send(query,ct));
    }

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.Dispatches.View,PermissionCodes.Dispatches.Query,PermissionCodes.Dispatches.Update)]
    public async Task<IActionResult> Detail(Guid id,CancellationToken ct)
    {
        return Ok(await sender.Send(new GetDispatchByIdQuery(id),ct));
    }

    /// <summary>Eski harici nüfus sorgusunun yerine yalnız yerel kayıt eşleştirmesi; sonucu cachelemeyiniz.</summary>
    [HttpPost("persons/lookup")]
    [HasPermission(PermissionCodes.Dispatches.Query)]
    public async Task<IActionResult> LookupPerson([FromBody] LookupDispatchPersonQuery query,CancellationToken ct)
    {
        var response=await sender.Send(query,ct);
        return response is null ? NotFound(new ProblemDetails{Title="Yerel kişi kaydı bulunamadı.",
            Detail="Kişi bilgilerini elle girerek yeni sevk oluşturabilirsiniz.",Status=404}) : Ok(response);
    }

    [HttpGet("options")]
    [HasPermission(PermissionCodes.Dispatches.View,PermissionCodes.Dispatches.Query,PermissionCodes.Dispatches.Create,PermissionCodes.Dispatches.Update)]
    public async Task<IActionResult> Options([FromQuery] GetDispatchOptionsQuery query,CancellationToken ct)
    {
        return Ok(await sender.Send(query,ct));
    }

    /// <summary>Seçili başvuru grubunun dinamik zorunluluklarını ve kullanılamayan muayene sebeplerini verir.</summary>
    [HttpGet("admission-rules/{admissionTypeId:guid}")]
    [HasPermission(PermissionCodes.Dispatches.View,PermissionCodes.Dispatches.Query,PermissionCodes.Dispatches.Create,PermissionCodes.Dispatches.Update)]
    public async Task<IActionResult> AdmissionRules(Guid admissionTypeId,CancellationToken ct)
    {
        return Ok(await sender.Send(new GetDispatchAdmissionRulesQuery(admissionTypeId),ct));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.Dispatches.Create)]
    public async Task<IActionResult> Create([FromBody] CreateDispatchCommand command,CancellationToken ct)
    {
        var created=await sender.Send(command,ct);
        return CreatedAtAction(nameof(Detail),new{id=created.Id},created);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.Dispatches.Update)]
    public async Task<IActionResult> Update(Guid id,[FromBody] UpdateDispatchCommand command,CancellationToken ct)
    {
        if(id!=command.Id)return BadRequest("Sevk kimliği uyuşmuyor.");
        return Ok(await sender.Send(command,ct));
    }

    [HttpPatch("{id:guid}/cancel")]
    [HasPermission(PermissionCodes.Dispatches.Update)]
    public async Task<IActionResult> Cancel(Guid id,[FromBody] CancelDispatchCommand command,CancellationToken ct)
    {
        if(id!=command.Id)return BadRequest("Sevk kimliği uyuşmuyor.");
        return Ok(await sender.Send(command,ct));
    }

    /// <summary>Başvuran fotoğrafını yetkili kullanıcıya cache olmadan gönderir.</summary>
    [HttpGet("{id:guid}/photo")]
    [HasPermission(PermissionCodes.Dispatches.View,PermissionCodes.Dispatches.Query,PermissionCodes.Dispatches.Update)]
    public async Task<IActionResult> Photo(Guid id,CancellationToken ct)
    {
        var photo=await sender.Send(new GetDispatchPhotoQuery(id),ct);
        Response.Headers["Cache-Control"]="no-store";
        Response.Headers["X-Content-Type-Options"]="nosniff";
        return File(photo.Content,photo.ContentType);
    }

    /// <summary>Kameradan çekilen veya seçilen resmi, en fazla 2 MiB, güvenli biçimde kaydeder.</summary>
    [HttpPost("{id:guid}/photo")]
    [HasPermission(PermissionCodes.Dispatches.Create,PermissionCodes.Dispatches.Update)]
    [RequestSizeLimit(3*1024*1024)]
    public async Task<IActionResult> UploadPhoto(Guid id,IFormFile file,CancellationToken ct)
    {
        if(file is null||file.Length is <1 or >2097152)
            return BadRequest("Fotoğraf en fazla 2 MiB olmalıdır.");
        await using var stream=new MemoryStream();
        await file.CopyToAsync(stream,ct);
        return Ok(await sender.Send(new SetDispatchPhotoCommand(id,file.ContentType,stream.ToArray()),ct));
    }

    /// <summary>Fotoğrafı sevk ve diğer belgeleri silmeden kaldırır; xmin versiyonunu denetler.</summary>
    [HttpDelete("{id:guid}/photo")]
    [HasPermission(PermissionCodes.Dispatches.Update)]
    public async Task<IActionResult> DeletePhoto(Guid id,[FromQuery] uint version,CancellationToken ct)
    {
        return Ok(await sender.Send(new DeleteDispatchPhotoCommand(id,version),ct));
    }

    [HttpGet("{id:guid}/files")]
    [HasPermission(PermissionCodes.Dispatches.View,PermissionCodes.Dispatches.Query,PermissionCodes.Dispatches.Update)]
    public async Task<IActionResult> Files(Guid id,CancellationToken ct)
    {
        return Ok(await sender.Send(new GetDispatchFilesQuery(id),ct));
    }

    [HttpGet("{id:guid}/files/{fileId:guid}")]
    [HasPermission(PermissionCodes.Dispatches.View,PermissionCodes.Dispatches.Query,PermissionCodes.Dispatches.Update)]
    public async Task<IActionResult> Download(Guid id,Guid fileId,CancellationToken ct)
    {
        var item=await sender.Send(new GetDispatchFileQuery(id,fileId),ct);
        Response.Headers["Cache-Control"]="no-store";
        Response.Headers["X-Content-Type-Options"]="nosniff";
        return File(item.Content,item.ContentType,item.FileName);
    }

    /// <summary>Önceden kaydedilmiş sevke yalnız PDF/PNG/JPEG en fazla 5 MiB yüklenir.</summary>
    [HttpPost("{id:guid}/files")]
    [HasPermission(PermissionCodes.Dispatches.Create,PermissionCodes.Dispatches.Update)]
    [RequestSizeLimit(6*1024*1024)]
    public async Task<IActionResult> Upload(Guid id,IFormFile file,CancellationToken ct)
    {
        if(file is null||file.Length is <1 or >5242880)
            return BadRequest("Dosya 1 byte ile 5 MiB arasında olmalıdır.");
        await using var stream=new MemoryStream();
        await file.CopyToAsync(stream,ct);
        var uploaded=await sender.Send(new UploadDispatchFileCommand(id,file.FileName,
            file.ContentType,stream.ToArray()),ct);
        return Ok(uploaded);
    }
}

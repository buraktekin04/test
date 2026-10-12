using KLMN.Api.Authorization;
using KLMN.Application.Users.Commands.ChangeUserStatus;
using KLMN.Application.Users.Agreements.Commands.UploadUserAgreementPdf;
using KLMN.Application.Users.Agreements.Commands.DeleteUserAgreementPdf;
using KLMN.Application.Users.Agreements.Queries.GetUserAgreementPdfs;
using KLMN.Application.Users.Agreements.Queries.DownloadUserAgreementPdf;
using KLMN.Application.Users.Agreements.Responses;
using KLMN.Application.Users.Commands.CreateUser;
using KLMN.Application.Users.Commands.DeleteUser;
using KLMN.Application.Users.Commands.UpdateUser;
using KLMN.Application.Users.Queries.GetUserById;
using KLMN.Application.Users.Queries.GetUsers;
using KLMN.Application.Users.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>
/// Kullanıcı tanımları ekranındaki listeleme, detay, oluşturma,
/// düzenleme, aktiflik ve soft delete işlemlerini sağlar.
/// </summary>
[ApiController]
[Route("api/users")]
public sealed class UsersController(ISender sender) : ControllerBase
{
    /// <summary>Yetki kontrollü ve sayfalı kullanıcı listesi döndürür.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.Users.Query)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] GetUsersQuery query, CancellationToken cancellationToken) =>
        Ok(await sender.Send(query, cancellationToken));

    /// <summary>Seçilen kullanıcıyı düzenleme ekranı için getirir.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.Users.Query)]
    [ProducesResponseType(typeof(UserListItemResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserListItemResponse>> GetUserById(
        [FromRoute] Guid id, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetUserByIdQuery(id), cancellationToken));

    /// <summary>Parolası hash'lenmiş standart rol sahibi yeni kullanıcı oluşturur.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.Users.Create)]
    [ProducesResponseType(typeof(UserMutationResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<UserMutationResponse>> CreateUser(
        [FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
    }

    /// <summary>Kullanıcı profilini optimistic concurrency korumasıyla günceller.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.Users.Update)]
    [ProducesResponseType(typeof(UserMutationResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserMutationResponse>> UpdateUser(
        [FromRoute] Guid id, [FromBody] UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest("Route ID ve request ID eşleşmiyor.");
        return Ok(await sender.Send(command, cancellationToken));
    }

    /// <summary>Kullanıcı hesabını aktifleştirir veya pasifleştirir.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.Users.Activate)]
    [ProducesResponseType(typeof(UserMutationResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserMutationResponse>> ChangeStatus(
        [FromRoute] Guid id, [FromBody] ChangeUserStatusCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest("Route ID ve request ID eşleşmiyor.");
        return Ok(await sender.Send(command, cancellationToken));
    }

    /// <summary>Kullanıcı kaydını fiziksel silmeden soft delete yapar.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.Users.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteUser(
        [FromRoute] Guid id, [FromQuery] uint version,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteUserCommand(id, version), cancellationToken);
        return NoContent();
    }

    /// <summary>Kullanıcıya bağlı PDF dokümanlarının meta verilerini listeler.</summary>
    [HttpGet("{id:guid}/agreements")]
    [HasPermission(PermissionCodes.Users.ViewDocuments)]
    public async Task<IActionResult> GetAgreements(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var documents = await sender.Send(
            new GetUserAgreementPdfsQuery(id), cancellationToken);
        return Ok(documents);
    }

    /// <summary>
    /// PDF içeriğini özel disk klasörüne, meta verilerini veritabanına kaydeder.
    /// Multipart form dosyasının kendisi PostgreSQL'e yazılmaz.
    /// </summary>
    [HttpPost("{id:guid}/agreements")]
    [Consumes("multipart/form-data")]
    [HasPermission(PermissionCodes.Users.ManageDocuments)]
    [ProducesResponseType(typeof(UserAgreementPdfResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<UserAgreementPdfResponse>> UploadAgreement(
        [FromRoute] Guid id, [FromForm] UploadAgreementPdfForm form,
        CancellationToken cancellationToken)
    {
        if (form.File is null || form.File.Length == 0)
            return BadRequest("Yüklenecek PDF dosyası zorunludur.");

        // Tarayıcının bildirdiği path bilgisi kullanılmaz; sadece görünür dosya adı alınır.
        var originalName = Path.GetFileName(form.File.FileName.Replace('\\', '/'));

        // Dosya uzantısı ve gerçek PDF başlığı ayrıca dosya deposunda kontrol edilir.
        if (!string.Equals(Path.GetExtension(originalName), ".pdf", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Yalnızca PDF uzantılı dosya yüklenebilir.");

        await using var uploadStream = form.File.OpenReadStream();
        var result = await sender.Send(new UploadUserAgreementPdfCommand(
            id, originalName, form.File.Length, uploadStream), cancellationToken);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// PDF'yi kullanıcı kimliği ve doküman kimliğiyle yetkili şekilde sunar.
    /// Direkt disk yolu veya wwwroot URL'si hiçbir zaman dışarı verilmez.
    /// </summary>
    [HttpGet("{id:guid}/agreements/{documentId:guid}/download")]
    [HasPermission(PermissionCodes.Users.ViewDocuments)]
    public async Task<IActionResult> DownloadAgreement(
        [FromRoute] Guid id, [FromRoute] Guid documentId,
        CancellationToken cancellationToken)
    {
        var file = await sender.Send(
            new DownloadUserAgreementPdfQuery(id, documentId), cancellationToken);

        // ASP.NET Core yanıt akışını gönderim tamamlandığında kendisi dispose eder.
        return File(file.Content, "application/pdf", file.OriginalFileName);
    }

    /// <summary>PDF meta verisini soft delete yapar ve disk dosyasını kaldırır.</summary>
    [HttpDelete("{id:guid}/agreements/{documentId:guid}")]
    [HasPermission(PermissionCodes.Users.ManageDocuments)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAgreement(
        [FromRoute] Guid id, [FromRoute] Guid documentId,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteUserAgreementPdfCommand(id, documentId), cancellationToken);
        return NoContent();
    }
}

/// <summary>Multipart PDF yükleme formunun sunucu binding modelidir.</summary>
public sealed class UploadAgreementPdfForm
{
    /// <summary>PDF içeriğini taşıyan zorunlu dosya alanıdır (form-data: file).</summary>
    public IFormFile File { get; set; } = null!;
}

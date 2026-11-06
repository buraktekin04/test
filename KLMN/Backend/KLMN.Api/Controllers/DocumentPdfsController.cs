using KLMN.Api.Authorization;
using KLMN.Application.DocumentPdfs.Commands.DeleteDocumentPdf;
using KLMN.Application.DocumentPdfs.Commands.UploadDocumentPdf;
using KLMN.Application.DocumentPdfs.Queries.GetDocumentPdfFile;
using KLMN.Application.DocumentPdfs.Queries.GetDocumentPdfs;
using KLMN.Application.DocumentPdfs.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Yönetmelik PDF listeleme, güvenli görüntüleme, yükleme ve silme API'si.</summary>
[ApiController]
[Route("api/document-pdfs")]
public sealed class DocumentPdfsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [HasPermission(PermissionCodes.DocumentPdfs.View, PermissionCodes.DocumentPdfs.Manage)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
        => Ok(await sender.Send(new GetDocumentPdfsQuery(), cancellationToken));

    [HttpGet("{id:guid}/file")]
    [HasPermission(PermissionCodes.DocumentPdfs.View, PermissionCodes.DocumentPdfs.Manage)]
    public async Task<IActionResult> ViewPdf(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetDocumentPdfFileQuery(id), cancellationToken);
        Response.Headers.CacheControl = "no-store";
        Response.Headers["X-Content-Type-Options"] = "nosniff";
        return File(result.Content, "application/pdf");
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(52 * 1024 * 1024)]
    [HasPermission(PermissionCodes.DocumentPdfs.Manage)]
    [ProducesResponseType(typeof(DocumentPdfResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<DocumentPdfResponse>> Upload(
        [FromForm] DocumentPdfUploadForm form, CancellationToken cancellationToken)
    {
        if (form.File is null || form.File.Length == 0)
            return BadRequest("Yüklenecek PDF dosyası zorunludur.");
        var name = Path.GetFileName(form.File.FileName.Replace('\\', '/'));
        if (string.IsNullOrWhiteSpace(name) ||
            !string.Equals(Path.GetExtension(name), ".pdf", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Yalnızca PDF uzantılı dosyalar yüklenebilir.");

        await using var stream = form.File.OpenReadStream();
        var created = await sender.Send(
            new UploadDocumentPdfCommand(name, form.File.Length, stream), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.DocumentPdfs.Manage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(
        Guid id, [FromQuery] uint version, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteDocumentPdfCommand(id, version), cancellationToken);
        return NoContent();
    }
}

/// <summary>Yeni PDF dosyası multipart form alanı.</summary>
public sealed class DocumentPdfUploadForm
{
    public IFormFile File { get; set; } = null!;
}


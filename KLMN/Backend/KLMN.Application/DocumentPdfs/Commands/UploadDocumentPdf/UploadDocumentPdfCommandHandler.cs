using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Interfaces.Storage;
using KLMN.Application.DocumentPdfs.Responses;
using KLMN.Domain.Entities.Documents;
using MediatR;

namespace KLMN.Application.DocumentPdfs.Commands.UploadDocumentPdf;

/// <summary>PDF'yi özel diske kaydeder, GUID ilişkisini DB'de korur.</summary>
public sealed class UploadDocumentPdfCommandHandler(IKLMNDbContext db, IDocumentPdfStorage storage)
    : IRequestHandler<UploadDocumentPdfCommand, DocumentPdfResponse>
{
    public async Task<DocumentPdfResponse> Handle(
        UploadDocumentPdfCommand request, CancellationToken cancellationToken)
    {
        if (request.SizeBytes > storage.MaxFileSizeBytes)
            throw new InvalidDataException("PDF izin verilen dosya boyutunu aşıyor.");
        var safeName = Path.GetFileName(request.FileName.Replace('\\', '/')).Trim();
        if (safeName.Length == 0 || safeName.Length > 255 ||
            !string.Equals(Path.GetExtension(safeName), ".pdf", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Geçersiz PDF dosya adı.");

        // Yeni içerik için yeni kimlik üretilir; aktarılan eski PDFDataObjectId değiştirilmeyecektir.
        var pdfDataObjectId = Guid.NewGuid();
        var fileSize = await storage.SaveAsync(pdfDataObjectId, request.Content, cancellationToken);
        try
        {
            var item = new DocumentPdf
            {
                FileName = safeName,
                PdfDataObjectId = pdfDataObjectId,
                SizeBytes = fileSize
            };
            await db.DocumentPdfs.AddAsync(item, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
            return new DocumentPdfResponse(
                item.Id, item.FileName, item.SizeBytes, item.CreatedDate, item.Version);
        }
        catch
        {
            await storage.DeleteAsync(pdfDataObjectId, CancellationToken.None);
            throw;
        }
    }
}


using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Interfaces.Storage;
using KLMN.Application.DocumentPdfs.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KLMN.Application.DocumentPdfs.Commands.DeleteDocumentPdf;

/// <summary>Dokümanı soft delete eder; işlem sonrası özel disk dosyasını temizler.</summary>
public sealed class DeleteDocumentPdfCommandHandler(
    IKLMNDbContext db, IDocumentPdfStorage storage,
    ILogger<DeleteDocumentPdfCommandHandler> logger)
    : IRequestHandler<DeleteDocumentPdfCommand>
{
    public async Task Handle(DeleteDocumentPdfCommand request, CancellationToken cancellationToken)
    {
        var document = await db.DocumentPdfs.FirstOrDefaultAsync(
            x => x.Id == request.Id, cancellationToken)
            ?? throw new DocumentPdfNotFoundException();

        if (document.Version != request.Version)
            throw new DocumentPdfConflictException();

        db.DocumentPdfs.Remove(document);
        await db.SaveChangesAsync(cancellationToken);
        try
        {
            await storage.DeleteAsync(document.PdfDataObjectId, cancellationToken);
        }
        catch (IOException ex)
        {
            logger.LogWarning(ex,
                "Silinmiş dokümanın fiziksel PDF dosyası temizlenemedi. DocumentId={DocumentId}",
                document.Id);
        }
    }
}


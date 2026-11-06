using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Interfaces.Storage;
using KLMN.Application.DocumentPdfs.Exceptions;
using KLMN.Application.DocumentPdfs.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.DocumentPdfs.Queries.GetDocumentPdfFile;

/// <summary>Dosya anahtarını yalnızca aktif kayıt üzerinden elde eder.</summary>
public sealed class GetDocumentPdfFileQueryHandler(IKLMNDbContext db, IDocumentPdfStorage storage)
    : IRequestHandler<GetDocumentPdfFileQuery, DocumentPdfFileResponse>
{
    public async Task<DocumentPdfFileResponse> Handle(
        GetDocumentPdfFileQuery request, CancellationToken cancellationToken)
    {
        var objectId = await db.DocumentPdfs.AsNoTracking()
            .Where(x => x.Id == request.Id && x.IsActive)
            .Select(x => (Guid?)x.PdfDataObjectId)
            .SingleOrDefaultAsync(cancellationToken);

        if (objectId is null) throw new DocumentPdfNotFoundException();
        try
        {
            return new DocumentPdfFileResponse(
                await storage.OpenReadAsync(objectId.Value, cancellationToken));
        }
        catch (FileNotFoundException) { throw new DocumentPdfNotFoundException(); }
        catch (DirectoryNotFoundException) { throw new DocumentPdfNotFoundException(); }
    }
}


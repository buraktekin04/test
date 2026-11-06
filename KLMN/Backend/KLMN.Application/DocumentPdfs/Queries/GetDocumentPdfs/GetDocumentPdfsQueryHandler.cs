using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.DocumentPdfs.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.DocumentPdfs.Queries.GetDocumentPdfs;

/// <summary>Görüntülenebilir belgeleri dosya adı ve yükleme tarihine göre sıralar.</summary>
public sealed class GetDocumentPdfsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetDocumentPdfsQuery, IReadOnlyList<DocumentPdfResponse>>
{
    public async Task<IReadOnlyList<DocumentPdfResponse>> Handle(
        GetDocumentPdfsQuery request, CancellationToken cancellationToken)
        => await db.DocumentPdfs.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.FileName).ThenByDescending(x => x.CreatedDate)
            .Select(x => new DocumentPdfResponse(
                x.Id, x.FileName, x.SizeBytes, x.CreatedDate, x.Version))
            .ToListAsync(cancellationToken);
}


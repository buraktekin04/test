using KLMN.Application.DocumentPdfs.Responses;
using MediatR;

namespace KLMN.Application.DocumentPdfs.Queries.GetDocumentPdfs;

/// <summary>Menüde ve yönetimde gösterilecek aktif PDF listesi.</summary>
public sealed record GetDocumentPdfsQuery() : IRequest<IReadOnlyList<DocumentPdfResponse>>;


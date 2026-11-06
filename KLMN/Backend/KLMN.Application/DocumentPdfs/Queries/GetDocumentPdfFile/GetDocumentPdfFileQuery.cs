using KLMN.Application.DocumentPdfs.Responses;
using MediatR;

namespace KLMN.Application.DocumentPdfs.Queries.GetDocumentPdfFile;

/// <summary>Kimliği verilen yönetmeliğin PDF içeriğini açar.</summary>
public sealed record GetDocumentPdfFileQuery(Guid Id) : IRequest<DocumentPdfFileResponse>;


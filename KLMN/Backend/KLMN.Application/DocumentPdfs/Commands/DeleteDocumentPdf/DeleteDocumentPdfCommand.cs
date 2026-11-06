using MediatR;

namespace KLMN.Application.DocumentPdfs.Commands.DeleteDocumentPdf;

/// <summary>xmin sürümüyle korunan doküman silme komutu.</summary>
public sealed record DeleteDocumentPdfCommand(Guid Id, uint Version) : IRequest;


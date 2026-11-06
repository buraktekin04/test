using KLMN.Application.DocumentPdfs.Responses;
using MediatR;

namespace KLMN.Application.DocumentPdfs.Commands.UploadDocumentPdf;

/// <summary>Yeni yönetmelik PDF yükleme komutu.</summary>
public sealed record UploadDocumentPdfCommand(string FileName, long SizeBytes, Stream Content)
    : IRequest<DocumentPdfResponse>;


namespace KLMN.Application.DocumentPdfs.Responses;

/// <summary>Yetkilendirilmiş PDF görüntüleme yanıtının akış modelidir.</summary>
public sealed record DocumentPdfFileResponse(Stream Content);


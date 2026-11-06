namespace KLMN.Application.DocumentPdfs.Responses;

/// <summary>Menü ve yönetim ekranı için PDF içerik veya fiziksel yol içermeyen model.</summary>
public sealed record DocumentPdfResponse(
    Guid Id, string FileName, long SizeBytes, DateTime CreatedDate, uint Version);


using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Documents;

/// <summary>Eski MSKBS DocumentPdf modelinin GUID alanlarını koruyan yönetmelik PDF kaydı.</summary>
public sealed class DocumentPdf : BaseEntity
{
    /// <summary>Doküman listesindeki görünen PDF dosya adı.</summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>Eski PDFDataObjectId kimliği; disk üzerindeki güvenli içerik anahtarıdır.</summary>
    public Guid PdfDataObjectId { get; set; }

    /// <summary>Dosyanın byte cinsinden boyutu.</summary>
    public long SizeBytes { get; set; }
}


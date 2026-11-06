namespace KLMN.Application.DocumentPdfs.Exceptions;

/// <summary>Silinmiş veya bulunamayan yönetmelik PDF kaydı.</summary>
public sealed class DocumentPdfNotFoundException : Exception
{
    public DocumentPdfNotFoundException() : base("Doküman bulunamadı.") { }
}


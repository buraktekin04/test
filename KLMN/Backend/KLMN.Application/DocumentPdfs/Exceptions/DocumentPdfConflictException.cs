namespace KLMN.Application.DocumentPdfs.Exceptions;

/// <summary>Dokümanın eşzamanlı değişiklik nedeniyle işlenememesi.</summary>
public sealed class DocumentPdfConflictException : Exception
{
    public DocumentPdfConflictException() : base("Doküman başka bir işlem tarafından değiştirildi. Listeyi yenileyin.") { }
}


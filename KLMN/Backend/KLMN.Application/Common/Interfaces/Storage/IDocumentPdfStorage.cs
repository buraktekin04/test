namespace KLMN.Application.Common.Interfaces.Storage;

/// <summary>Yönetmelik PDF binary içeriğinin web kökü dışındaki depoda tutulması.</summary>
public interface IDocumentPdfStorage
{
    long MaxFileSizeBytes { get; }
    Task<long> SaveAsync(Guid pdfDataObjectId, Stream content, CancellationToken cancellationToken);
    Task<Stream> OpenReadAsync(Guid pdfDataObjectId, CancellationToken cancellationToken);
    Task DeleteAsync(Guid pdfDataObjectId, CancellationToken cancellationToken);
}


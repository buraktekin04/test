using KLMN.Application.Common.Interfaces.Storage;
using KLMN.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace KLMN.Infrastructure.Storage;

/// <summary>Yönetmelik dosyalarını kullanıcı PDF'lerinden ayrı private regulations dizininde saklar.</summary>
public sealed class LocalDocumentPdfStorage : IDocumentPdfStorage
{
    private readonly string _root;
    public long MaxFileSizeBytes { get; }

    public LocalDocumentPdfStorage(IOptions<DocumentStorageSettings> settings)
    {
        var configured = Path.GetFullPath(
            Environment.ExpandEnvironmentVariables(settings.Value.RootPath));
        if (configured.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            .Any(s => string.Equals(s, "wwwroot", StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Yönetmelik PDF dosyaları wwwroot altında tutulamaz.");
        _root = Path.Combine(configured, "regulations");
        MaxFileSizeBytes = settings.Value.MaxFileSizeMb * 1024L * 1024L;
    }

    private string FilePath(Guid id)
    {
        if (id == Guid.Empty) throw new InvalidDataException("PDF veri kimliği geçersiz.");
        return Path.Combine(_root, id.ToString("N") + ".pdf");
    }

    public async Task<long> SaveAsync(Guid pdfDataObjectId, Stream content, CancellationToken cancellationToken)
    {
        var path = FilePath(pdfDataObjectId);
        Directory.CreateDirectory(_root);
        try
        {
            await using var dest = new FileStream(path, FileMode.CreateNew, FileAccess.Write,
                FileShare.None, 81920, FileOptions.Asynchronous);
            var magic = new byte[5];
            var length = 0;
            while (length < magic.Length)
            {
                var n = await content.ReadAsync(magic.AsMemory(length), cancellationToken);
                if (n == 0) break;
                length += n;
            }
            if (length != 5 || magic[0] != 37 || magic[1] != 80 ||
                magic[2] != 68 || magic[3] != 70 || magic[4] != 45)
                throw new InvalidDataException("Yalnızca geçerli PDF dosyası yüklenebilir.");

            await dest.WriteAsync(magic, cancellationToken);
            long bytes = magic.Length;
            var buffer = new byte[81920];
            int read;
            while ((read = await content.ReadAsync(buffer, cancellationToken)) > 0)
            {
                bytes += read;
                if (bytes > MaxFileSizeBytes)
                    throw new InvalidDataException("PDF izin verilen dosya boyutunu aşıyor.");
                await dest.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            }
            return bytes;
        }
        catch
        {
            if (File.Exists(path)) File.Delete(path);
            throw;
        }
    }

    public Task<Stream> OpenReadAsync(Guid pdfDataObjectId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Stream stream = new FileStream(FilePath(pdfDataObjectId), FileMode.Open,
            FileAccess.Read, FileShare.Read, 81920,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(Guid pdfDataObjectId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var path = FilePath(pdfDataObjectId);
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }
}


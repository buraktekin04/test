using KLMN.Application.Common.Interfaces.Storage;
using KLMN.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace KLMN.Infrastructure.Storage;

/// <summary>
/// PDF dosyalarını web root dışında bir klasörde güvenli GUID dosya
/// adlarıyla saklar. DB'ye içerik değil yalnızca göreli anahtar yazılır.
/// </summary>
public sealed class LocalUserAgreementPdfStorage : IUserAgreementPdfStorage
{
    /// <summary>Fiziksel özel dosya saklama kök klasörüdür.</summary>
    private readonly string _rootPath;
    /// <summary>Bir PDF'nin izin verilen en büyük byte uzunluğudur.</summary>
    public long MaxFileSizeBytes { get; }

    /// <summary>Ortam değişkenlerini çözer; kök klasörün tam yolunu hesaplar.</summary>
    public LocalUserAgreementPdfStorage(IOptions<DocumentStorageSettings> settings)
    {
        var options = settings.Value;
        var expanded = Environment.ExpandEnvironmentVariables(options.RootPath);
        _rootPath = Path.GetFullPath(expanded);

        // Yanlış yapılandırmayla dosyaları herkese açık wwwroot altına yazmayız.
        var segments = _rootPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        if (segments.Any(s => s.Equals("wwwroot", StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("PDF dosyaları wwwroot altında saklanamaz.");

        MaxFileSizeBytes = options.MaxFileSizeMb * 1024L * 1024L;
    }

    /// <summary>Akıştan en fazla izin verilen boyutta PDF okur ve diske yazar.</summary>
    public async Task<StoredUserAgreementPdf> SaveAsync(
        Guid userId, Stream content, CancellationToken cancellationToken)
    {
        // Dosya adını istemciden almayız; traversal ve mevcut dosyanın ezilmesi önlenir.
        var relativePath = Path.Combine(userId.ToString("N"), Guid.NewGuid().ToString("N") + ".pdf");
        var filePath = GetSafeFullPath(relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        try
        {
            await using var destination = new FileStream(
                filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                bufferSize: 81920, options: FileOptions.Asynchronous);

            // Dosyanın en az PDF başlığı (%PDF-) taşımasını zorunlu tutar.
            var magic = new byte[5];
            var read = 0;
            while (read < magic.Length)
            {
                var count = await content.ReadAsync(
                    magic.AsMemory(read, magic.Length - read), cancellationToken);
                if (count == 0) break;
                read += count;
            }

            if (read != magic.Length ||
                magic[0] != 0x25 || magic[1] != 0x50 ||
                magic[2] != 0x44 || magic[3] != 0x46 || magic[4] != 0x2D)
            {
                throw new InvalidDataException("Yüklenen dosya geçerli bir PDF başlığı içermiyor.");
            }

            await destination.WriteAsync(magic, cancellationToken);

            var buffer = new byte[81920];
            long total = magic.Length;
            int length;
            while ((length = await content.ReadAsync(buffer, cancellationToken)) > 0)
            {
                total += length;
                if (total > MaxFileSizeBytes)
                    throw new InvalidDataException("PDF izin verilen dosya boyutunu aşıyor.");

                await destination.WriteAsync(buffer.AsMemory(0, length), cancellationToken);
            }

            return new StoredUserAgreementPdf(relativePath.Replace('\\', '/'), total);
        }
        catch
        {
            if (File.Exists(filePath)) File.Delete(filePath);
            throw;
        }
    }

    /// <summary>Yalnızca özel kök dizindeki dosyayı salt okunur akış olarak açar.</summary>
    public Task<Stream> OpenReadAsync(string relativePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Stream file = new FileStream(GetSafeFullPath(relativePath),
            FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 81920, options: FileOptions.Asynchronous | FileOptions.SequentialScan);
        return Task.FromResult(file);
    }

    /// <summary>Veritabanı kaydı kaldırılmış PDF dosyasını fiziksel olarak siler.</summary>
    public Task DeleteAsync(string relativePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var file = GetSafeFullPath(relativePath);
        if (File.Exists(file)) File.Delete(file);
        return Task.CompletedTask;
    }

    /// <summary>Kötü amaçlı göreli anahtarların kök klasör dışına çıkmasını engeller.</summary>
    private string GetSafeFullPath(string relativePath)
    {
        if (Path.IsPathRooted(relativePath))
            throw new InvalidDataException("Geçersiz dosya konumu.");

        var fullPath = Path.GetFullPath(Path.Combine(_rootPath, relativePath));
        var allowedPrefix = _rootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        if (!fullPath.StartsWith(allowedPrefix, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Dosya konumu izin verilen klasör dışında.");

        return fullPath;
    }
}

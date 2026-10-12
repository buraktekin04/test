namespace KLMN.Application.Common.Interfaces.Storage;

/// <summary>Diske yazılan PDF'nin byte boyutu ve güvenli göreli dosya anahtarıdır.</summary>
public sealed record StoredUserAgreementPdf(string RelativePath, long SizeBytes);

/// <summary>
/// Kullanıcı PDF içeriğini private dosya sistemine kaydetme/okuma/silme
/// sözleşmesidir. Application katmanı fiziksel disk yolunu bilmez.
/// </summary>
public interface IUserAgreementPdfStorage
{
    /// <summary>Uygulama ayarlarından hesaplanan en büyük dosya uzunluğudur.</summary>
    long MaxFileSizeBytes { get; }

    /// <summary>
    /// PDF imzasını kontrol eder ve rastgele dosya adıyla kullanıcı dizinine yazar.
    /// </summary>
    Task<StoredUserAgreementPdf> SaveAsync(
        Guid userId, Stream content, CancellationToken cancellationToken);

    /// <summary>
    /// DB'de saklanan güvenli göreli anahtar üzerinden PDF dosyasını açar.
    /// </summary>
    Task<Stream> OpenReadAsync(string relativePath, CancellationToken cancellationToken);

    /// <summary>
    /// Yalnızca kök klasörün içindeki belirtilen dosyayı siler.
    /// </summary>
    Task DeleteAsync(string relativePath, CancellationToken cancellationToken);
}

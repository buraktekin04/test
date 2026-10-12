namespace KLMN.Application.Users.Agreements.Responses;

/// <summary>Disk yolu paylaşmadan kullanıcı PDF dokümanının metadata bilgisini döndürür.</summary>
public sealed record UserAgreementPdfResponse
{
    /// <summary>Doküman benzersiz kimliğidir.</summary>
    public required Guid Id { get; init; }
    /// <summary>Belgenin bağlı olduğu kullanıcı kimliğidir.</summary>
    public required Guid UserId { get; init; }
    /// <summary>İndirme sırasında önerilecek özgün PDF dosya adıdır.</summary>
    public required string FileName { get; init; }
    /// <summary>PDF'nin byte uzunluğudur.</summary>
    public required long SizeBytes { get; init; }
    /// <summary>Yüklemenin gerçekleştiği UTC zamandır.</summary>
    public required DateTime CreatedDate { get; init; }
}

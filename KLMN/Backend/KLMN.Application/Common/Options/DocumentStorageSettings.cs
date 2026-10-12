namespace KLMN.Application.Common.Options;

/// <summary>PDF içeriklerinin diskte tutulacağı kök dizini ve boyut limitini tanımlar.</summary>
public sealed class DocumentStorageSettings
{
    /// <summary>appsettings'teki bölüm adıdır.</summary>
    public const string SectionName = "DocumentStorage";
    /// <summary>
    /// Development'ta kullanıcının masaüstü klasörü; canlıda sunucudaki
    /// yazılabilir, web root dışında özel dizindir.
    /// </summary>
    public string RootPath { get; init; } = string.Empty;
    /// <summary>PDF başına izin verilen en büyük dosya boyutudur (MB).</summary>
    public int MaxFileSizeMb { get; init; } = 10;
}

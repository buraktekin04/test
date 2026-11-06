namespace KLMN.Domain.Entities.Admissions;

/// <summary>Provizyon durumlarının merkezi ve salt okunur tanım kaydı.</summary>
/// <remarks>Kod, mevcut provizyon kayıtlarıyla uyumlu doğal anahtardır.</remarks>
public sealed class ProvisionStatusDef
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

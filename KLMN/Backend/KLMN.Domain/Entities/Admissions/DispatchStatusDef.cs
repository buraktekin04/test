namespace KLMN.Domain.Entities.Admissions;

/// <summary>Sevk durumlarının DB'de saklanan sabit sözlük kayıtları. Yönetim ekranı henüz yoktur.</summary>
/// <remarks>Dispatch.CurrentState ile aynı numeric kodu taşır; lookup kaydı audit alanları gerektirmez.</remarks>
public sealed class DispatchStatusDef
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

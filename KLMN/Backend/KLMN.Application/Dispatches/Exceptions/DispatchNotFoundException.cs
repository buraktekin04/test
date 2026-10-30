namespace KLMN.Application.Dispatches.Exceptions;

/// <summary>Sevk veya ona ait belge bulunamadığında kullanılan özel hata.</summary>
public sealed class DispatchNotFoundException : Exception
{
    public DispatchNotFoundException() : base("Sevk kaydı bulunamadı.") { }
}

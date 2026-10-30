namespace KLMN.Application.Dispatches.Exceptions;

/// <summary>Nüfus entegrasyonu henüz kurum servislerine bağlanmadığında dönen kontrollü hata.</summary>
public sealed class NationalRegistryNotConfiguredException : Exception
{
    public NationalRegistryNotConfiguredException()
        : base("Nüfus kimlik sorgulama servisi henüz yapılandırılmadı.")
    {
    }
}

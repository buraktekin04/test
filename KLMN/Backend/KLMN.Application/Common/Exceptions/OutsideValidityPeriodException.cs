namespace KLMN.Application.Common.Exceptions;

/// <summary>
/// Hesabın tanımlı geçerlilik dönemi dışında yapılan giriş, refresh
/// veya JWT isteklerini HTTP 403 ile reddetmek için kullanılır.
/// </summary>
public sealed class OutsideValidityPeriodException : Exception
{
    /// <summary>Geçerlilik başlangıç/bitiş tarihi uyarısını üretir.</summary>
    public OutsideValidityPeriodException()
        : base("Hesabınızın kullanım başlangıç/bitiş tarihleri dışında erişim izniniz bulunmuyor. Yöneticinizle görüşünüz.")
    {
    }
}

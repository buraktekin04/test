namespace KLMN.Domain.Common;

/// <summary>
/// Kullanıcı ve gelecekteki tanım entity'leri için ortak UTC geçerlilik
/// aralığı karşılaştırmasıdır. Aralık [ValidFrom, ValidTo) olarak yorumlanır.
/// </summary>
public static class ValidityPeriod
{
    /// <summary>
    /// Her iki tarih de tanımlıysa başlangıcın bitişten kesin önce olmasını
    /// gerektirir. Açık uçlu/null sınırlar geçerlidir.
    /// </summary>
    public static bool HasValidBounds(DateTime? validFrom, DateTime? validTo) =>
        !validFrom.HasValue || !validTo.HasValue || validFrom.Value < validTo.Value;

    /// <summary>
    /// Verilen UTC anının başlangıç dahil, bitiş hariç zaman aralığında
    /// bulunup bulunmadığını belirtir. Geçersiz ters aralıkta izin vermez.
    /// </summary>
    public static bool IsWithin(DateTime? validFrom, DateTime? validTo, DateTime utcNow) =>
        HasValidBounds(validFrom, validTo) &&
        (!validFrom.HasValue || validFrom.Value <= utcNow) &&
        (!validTo.HasValue || validTo.Value > utcNow);
}

namespace KLMN.Domain.Common;

/// <summary>
/// Tanım kayıtlarında tekrar kullanılacak isteğe bağlı geçerlilik
/// tarihlerini BaseEntity'den kalıtımla taşır.
/// Başlangıç dahil, bitiş hariçtir; null sınırın olmadığı anlamına gelir.
/// Bu sınıftan türeyen entity'ler PostgreSQL'de fiziksel olarak iki
/// ayrı timestamp with time zone kolonu kullanır.
/// </summary>
public abstract class EffectiveDatedEntity : BaseEntity
{
    /// <summary>
    /// Kaydın geçerliliğinin başladığı UTC andır (dahil).
    /// Null: geçmişe yönelik bir başlangıç kısıtı bulunmaz.
    /// </summary>
    public DateTime? ValidFrom { get; set; }

    /// <summary>
    /// Kaydın geçerliliğinin sona erdiği UTC andır (hariç).
    /// Null: geleceğe yönelik bir bitiş kısıtı bulunmaz.
    /// </summary>
    public DateTime? ValidTo { get; set; }

    /// <summary>
    /// Kaydın UTC zamanında kullanılabilir olduğu aralıkta bulunup
    /// bulunmadığını denetler. IsActive/IsDeleted gibi başka kuralları
    /// sorgulamaz; yalnızca tarih karşılaştırmasını yapar.
    /// </summary>
    public bool IsValidAt(DateTime utcNow) =>
        ValidityPeriod.IsWithin(ValidFrom, ValidTo, utcNow);
}

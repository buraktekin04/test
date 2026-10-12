namespace KLMN.Application.Common.Exceptions;

/// <summary>
/// Mesai dışı erişim onayı bulunmayan kullanıcı giriş/refresh işlemlerinde
/// güvenli ve açıklayıcı 403 yanıtı üretmek için kullanılan exception'dır.
/// </summary>
public sealed class OutsideWorkingHoursException : Exception
{
    /// <summary>Kullanıcıya gösterilecek erişim engeli mesajını oluşturur.</summary>
    public OutsideWorkingHoursException()
        : base("Mesai saatleri dışında uygulamaya erişim yetkiniz bulunmuyor. Yöneticinizle görüşünüz.")
    {
    }
}

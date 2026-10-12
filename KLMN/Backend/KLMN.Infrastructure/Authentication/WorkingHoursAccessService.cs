using System.Globalization;
using KLMN.Application.Common.Interfaces.Authentication;
using KLMN.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace KLMN.Infrastructure.Authentication;

/// <summary>
/// Türkiye saati gibi yapılandırılmış yerel saat diliminde mesai kuralını
/// kontrol eder. Sunucunun işletim sistemi saat dilimine bağımlı değildir.
/// </summary>
public sealed class WorkingHoursAccessService : IWorkingHoursAccessService
{
    /// <summary>Etkin ortamın çalışma günleri ve saat aralığıdır.</summary>
    private readonly WorkingHoursSettings _settings;
    /// <summary>Saat hesaplamalarını test edilebilir yapan UTC zaman kaynağıdır.</summary>
    private readonly TimeProvider _timeProvider;
    /// <summary>Mesainin uygulandığı kurum yerel zaman dilimidir.</summary>
    private readonly TimeZoneInfo _zone;

    /// <summary>Mesai ayarlarını, zaman kaynağını ve saat dilimini çözer.</summary>
    public WorkingHoursAccessService(
        IOptions<WorkingHoursSettings> settings,
        TimeProvider timeProvider)
    {
        _settings = settings.Value;
        _timeProvider = timeProvider;
        _zone = TimeZoneInfo.FindSystemTimeZoneById(_settings.TimeZoneId);
    }

    /// <summary>
    /// ClockLimit pasifse her zaman izin verir; aktifse yalnızca
    /// tanımlanan haftanın günlerinde başlangıç dahil bitiş hariç izin verir.
    /// </summary>
    public bool IsAllowed(bool clockLimit)
    {
        if (!clockLimit) return true;

        // UTC saatini kurumsal yerel saate dönüştürür, sunucu timezone'una bakmaz.
        var localTime = TimeZoneInfo.ConvertTime(_timeProvider.GetUtcNow(), _zone);

        // DayOfWeek Sunday=0 olduğu için ISO day değerine dönüştürülür.
        var day = localTime.DayOfWeek == DayOfWeek.Sunday
            ? 7 : (int)localTime.DayOfWeek;

        if (!_settings.DaysOfWeek.Contains(day)) return false;

        var start = TimeOnly.ParseExact(_settings.Start, "HH:mm", CultureInfo.InvariantCulture);
        var end = TimeOnly.ParseExact(_settings.End, "HH:mm", CultureInfo.InvariantCulture);
        var now = TimeOnly.FromDateTime(localTime.DateTime);

        return now >= start && now < end;
    }
}

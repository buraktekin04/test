namespace KLMN.Application.Common.Options;

/// <summary>Mesai saatlerini ve kurumun yerel saat dilimini appsettings'ten okur.</summary>
public sealed class WorkingHoursSettings
{
    /// <summary>JSON ayar bölümünün teknik adıdır.</summary>
    public const string SectionName = "WorkingHours";

    /// <summary>Mesai denetiminde kullanılan IANA/Windows zaman dilimidir.</summary>
    public string TimeZoneId { get; init; } = "Europe/Istanbul";

    /// <summary>Mesainin başladığı HH:mm saatidir (dahil).</summary>
    public string Start { get; init; } = "08:00";

    /// <summary>Mesainin sona erdiği HH:mm saatidir (hariç).</summary>
    public string End { get; init; } = "18:00";

    /// <summary>ISO haftanın günü: Pazartesi 1 ... Pazar 7.</summary>
    public int[] DaysOfWeek { get; init; } = [1, 2, 3, 4, 5];
}

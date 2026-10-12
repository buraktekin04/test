namespace KLMN.Application.Common.Interfaces.Authentication;

/// <summary>Kullanıcı mesai kısıtı için giriş ve JWT erişim kararı verir.</summary>
public interface IWorkingHoursAccessService
{
    /// <summary>
    /// ClockLimit=true ise yapılandırılmış yerel mesai aralığını kontrol eder.
    /// ClockLimit=false ise mesai dışı erişim izni verildiği için true döner.
    /// </summary>
    bool IsAllowed(bool clockLimit);
}

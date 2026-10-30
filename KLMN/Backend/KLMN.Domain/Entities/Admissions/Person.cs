using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Admissions;

/// <summary>Sevk başvurusunda kullanılan kişinin yerel kimlik ve iletişim kaydıdır; harici MERNİS yanıtı değildir.</summary>
public sealed class Person : BaseEntity
{
    /// <summary>11 haneli T.C. kimlik numarası; sayıya dönüştürülmez.</summary>
    public string? TcKimlikNo { get; set; }
    /// <summary>Yabancı kimlik numarası; sayıya dönüştürülmez.</summary>
    public string? YabanciKimlikNo { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string? MotherName { get; set; }
    public string? FatherName { get; set; }
    public Guid? NationalityId { get; set; }
    public Guid? TownOfBirthId { get; set; }
    public DateOnly BirthDate { get; set; }
    /// <summary>Eski sistemdeki sayısal cinsiyet kodu; doğrulanmadan enum varsayılmaz.</summary>
    public int? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? BirthPlaceText { get; set; }
    public string? BirthPlaceCode { get; set; }
    public ICollection<Dispatch> Dispatches { get; set; } = new List<Dispatch>();
}

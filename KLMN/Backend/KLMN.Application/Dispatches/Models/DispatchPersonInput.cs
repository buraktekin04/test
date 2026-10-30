namespace KLMN.Application.Dispatches.Models;

/// <summary>Sevk girişinde kullanıcı tarafından veya kurum nüfus entegrasyonu ile doldurulan kişi alanları.</summary>
public sealed class DispatchPersonInput
{
    public string? TcKimlikNo { get; set; }
    public string? YabanciKimlikNo { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string? MotherName { get; set; }
    public string? FatherName { get; set; }
    public Guid? NationalityId { get; set; }
    public Guid? TownOfBirthId { get; set; }
    public DateOnly BirthDate { get; set; }
    public int? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? BirthPlaceText { get; set; }
    public string? BirthPlaceCode { get; set; }
}

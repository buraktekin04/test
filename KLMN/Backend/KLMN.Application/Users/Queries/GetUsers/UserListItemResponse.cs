namespace KLMN.Application.Users.Queries.GetUsers;

/// <summary>
/// Kullanıcı listeleme ekranında döndürülen özet kullanıcı modelidir.
/// </summary>
public sealed class UserListItemResponse
{
    /// <summary>
    /// id özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// SMTP sunucusuna kimlik doğrulamada kullanılan hesap adıdır.
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// first name özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// last name özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Oturum sahibinin ad ve soyadının birleştirilmiş halidir.
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// email özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// phone number özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Token içerisinde kullanıcının bağlı olduğu birimin kimliğidir.
    /// </summary>
    public Guid? OrganizationUnitId { get; init; }

    /// <summary>
    /// organization unit code özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    public string? OrganizationUnitCode { get; init; }

    /// <summary>
    /// organization unit name özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    public string? OrganizationUnitName { get; init; }

    /// <summary>
    /// Oturum kullanıcısının role claim kodlarını içerir.
    /// </summary>
    public IReadOnlyCollection<string> Roles { get; init; } = [];

    /// <summary>
    /// is active özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// is locked özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    public bool IsLocked { get; init; }

    /// <summary>
    /// created date özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    public DateTime CreatedDate { get; init; }

    /// <summary>
    /// version özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    public uint Version { get; init; }
}

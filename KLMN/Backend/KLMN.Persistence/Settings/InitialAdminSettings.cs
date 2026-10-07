namespace KLMN.Persistence.Settings;

/// <summary>
/// İlk ADMIN kullanıcısının yalnızca ilk seed sırasında oluşturulması için
/// kullanılan configuration modelidir.
/// </summary>
public sealed class InitialAdminSettings
{
    public const string SectionName = "InitialAdmin";

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string FirstName { get; set; } = "System";

    public string LastName { get; set; } = "Administrator";
}

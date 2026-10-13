namespace KLMN.Application.Authentication.Responses;

/// <summary>
/// Kimliği doğrulanmış kullanıcının güncel profil ve authorization bilgisidir.
/// </summary>
public sealed record CurrentUserResponse
{
    /// <summary>
    /// Kullanıcı veya ilgili kaydın benzersiz kimliğidir.
    /// </summary>
    public required Guid Id { get; init; }
    /// <summary>
    /// Kullanıcının giriş sırasında kullandığı hesap adıdır.
    /// </summary>
    public required string UserName { get; init; }
    /// <summary>
    /// Hesabın doğrulama ve bildirim e-posta adresidir.
    /// </summary>
    public required string Email { get; init; }
    /// <summary>
    /// Kullanıcının adıdır.
    /// </summary>
    public required string FirstName { get; init; }
    /// <summary>
    /// Kullanıcının soyadıdır.
    /// </summary>
    public required string LastName { get; init; }
    /// <summary>
    /// Angular arayüzünde gösterilecek birleştirilmiş ad ve soyaddır.
    /// </summary>
    public required string FullName { get; init; }
    /// <summary>
    /// Kullanıcının isteğe bağlı telefon bilgisidir.
    /// </summary>
    public string? PhoneNumber { get; init; }
    /// <summary>
    /// Kullanıcının bağlı olduğu birliğin kimliğidir.
    /// </summary>
    public Guid? MilitaryUnitDefId { get; init; }
    /// <summary>
    /// Kullanıcının bağlı olduğu birliğin adıdır.
    /// </summary>
    public string? MilitaryUnitDefName { get; init; }
    /// <summary>
    /// last login date bilgisini ilgili veri sözleşmesinde taşır.
    /// </summary>
    public DateTime? LastLoginDate { get; init; }
    /// <summary>
    /// password changed date bilgisini ilgili veri sözleşmesinde taşır.
    /// </summary>
    public DateTime? PasswordChangedDate { get; init; }
    /// <summary>
    /// Kullanıcının etkin rol kodlarını içerir.
    /// </summary>
    public required IReadOnlyCollection<string> Roles { get; init; }
    /// <summary>
    /// Kullanıcının rol ve override kaynaklı etkin yetkilerini içerir.
    /// </summary>
    public required IReadOnlyCollection<string> Permissions { get; init; }
}

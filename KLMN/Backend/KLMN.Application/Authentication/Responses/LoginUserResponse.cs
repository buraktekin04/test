namespace KLMN.Application.Authentication.Responses;

/// <summary>
/// Login/refresh response içerisinde döndürülen kullanıcı bilgisidir.
/// </summary>
public sealed record LoginUserResponse
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

    /// <summary>Angular tarafında doğrudan gösterilebilecek tam addır.</summary>
    public required string FullName { get; init; }

    /// <summary>
    /// Kullanıcının bağlı olduğu birliğin kimliğidir.
    /// </summary>
    public Guid? MilitaryUnitDefId { get; init; }
    /// <summary>
    /// Kullanıcının etkin rol kodlarını içerir.
    /// </summary>
    public required IReadOnlyCollection<string> Roles { get; init; }
    /// <summary>
    /// Kullanıcının rol ve override kaynaklı etkin yetkilerini içerir.
    /// </summary>
    public required IReadOnlyCollection<string> Permissions { get; init; }
}

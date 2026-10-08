namespace KLMN.Application.Authentication.Responses;

/// <summary>
/// Başarılı kullanıcı girişinden sonra Angular istemcisine gönderilecek
/// authentication bilgilerini temsil eder.
/// </summary>
public sealed record LoginResponse
{
    public required string AccessToken { get; init; }
    public required DateTime AccessTokenExpiresAt { get; init; }
    public required LoginUserResponse User { get; init; }
}

/// <summary>
/// Login/refresh response içerisinde döndürülen kullanıcı bilgisidir.
/// </summary>
public sealed record LoginUserResponse
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    /// <summary>Angular tarafında doğrudan gösterilebilecek tam addır.</summary>
    public required string FullName { get; init; }

    public Guid? OrganizationUnitId { get; init; }
    public required IReadOnlyCollection<string> Roles { get; init; }
    public required IReadOnlyCollection<string> Permissions { get; init; }
}

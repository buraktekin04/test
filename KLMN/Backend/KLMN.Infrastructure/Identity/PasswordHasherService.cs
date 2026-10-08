using KLMN.Application.Common.Interfaces.Identity;
using Microsoft.AspNetCore.Identity;

namespace KLMN.Infrastructure.Identity;

/// <summary>ASP.NET Core Identity PasswordHasher tabanlı parola servisidir.</summary>
public sealed class PasswordHasherService : IPasswordHasherService
{
    /// <summary>
    /// ASP.NET Core Identity'nin güvenli parola hashleme hizmetidir.
    /// </summary>
    private readonly PasswordHasher<object> _passwordHasher = new();

    /// <summary>
    /// hash password işlemini ilgili katmanın sorumluluğuna göre gerçekleştirir.
    /// </summary>
    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        return _passwordHasher.HashPassword(
            user: null!,
            password);
    }

    /// <summary>
    /// verify password işlemini ilgili katmanın sorumluluğuna göre gerçekleştirir.
    /// </summary>
    public bool VerifyPassword(
        string hashedPassword,
        string providedPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword);
        ArgumentException.ThrowIfNullOrWhiteSpace(providedPassword);

        // Handler veya servis çağrısından elde edilen doğrulanmış işlem sonucudur.
        var result = _passwordHasher.VerifyHashedPassword(
            user: null!,
            hashedPassword,
            providedPassword);

        return result is
            PasswordVerificationResult.Success or
            PasswordVerificationResult.SuccessRehashNeeded;
    }
}

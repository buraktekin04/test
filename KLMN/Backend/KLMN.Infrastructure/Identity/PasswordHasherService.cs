using KLMN.Application.Common.Interfaces.Identity;
using Microsoft.AspNetCore.Identity;

namespace KLMN.Infrastructure.Identity;

/// <summary>ASP.NET Core Identity PasswordHasher tabanlı parola servisidir.</summary>
public sealed class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        return _passwordHasher.HashPassword(
            user: null!,
            password);
    }

    public bool VerifyPassword(
        string hashedPassword,
        string providedPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword);
        ArgumentException.ThrowIfNullOrWhiteSpace(providedPassword);

        var result = _passwordHasher.VerifyHashedPassword(
            user: null!,
            hashedPassword,
            providedPassword);

        return result is
            PasswordVerificationResult.Success or
            PasswordVerificationResult.SuccessRehashNeeded;
    }
}

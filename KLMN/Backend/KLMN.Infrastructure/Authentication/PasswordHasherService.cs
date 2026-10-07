using KLMN.Application.Common.Interfaces.Security;
using Microsoft.AspNetCore.Identity;

namespace KLMN.Infrastructure.Authentication;

/// <summary>
/// ASP.NET Core PasswordHasher kullanarak kullanıcı parolalarını yönetir.
/// </summary>
internal sealed class PasswordHasherService
    : IPasswordHasherService
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    /// <inheritdoc />
    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        return _passwordHasher.HashPassword(
            new object(),
            password);
    }

    /// <inheritdoc />
    public bool VerifyPassword(
        string passwordHash,
        string providedPassword)
    {
        if (string.IsNullOrWhiteSpace(passwordHash) ||
            string.IsNullOrWhiteSpace(providedPassword))
        {
            return false;
        }

        var result =
            _passwordHasher.VerifyHashedPassword(
                new object(),
                passwordHash,
                providedPassword);

        return result is
            PasswordVerificationResult.Success or
            PasswordVerificationResult.SuccessRehashNeeded;
    }
}

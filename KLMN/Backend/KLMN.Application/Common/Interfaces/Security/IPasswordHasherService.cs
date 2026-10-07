namespace KLMN.Application.Common.Interfaces.Security;

/// <summary>
/// Kullanıcı parolalarının hashlenmesi ve doğrulanmasını soyutlar.
/// </summary>
public interface IPasswordHasherService
{
    /// <summary>
    /// Açık paroladan güvenli hash üretir.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Açık parolayı mevcut hash ile doğrular.
    /// </summary>
    bool VerifyPassword(
        string passwordHash,
        string providedPassword);
}

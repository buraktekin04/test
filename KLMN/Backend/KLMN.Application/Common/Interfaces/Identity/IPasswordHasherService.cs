namespace KLMN.Application.Common.Interfaces.Identity;

/// <summary>Kullanıcı parolalarının hash'lenmesi ve doğrulanmasını soyutlar.</summary>
public interface IPasswordHasherService
{
    string HashPassword(string password);

    bool VerifyPassword(
        string hashedPassword,
        string providedPassword);
}

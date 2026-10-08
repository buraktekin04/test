namespace KLMN.Application.Common.Interfaces.Identity;

/// <summary>Kullanıcı parolalarının hash'lenmesi ve doğrulanmasını soyutlar.</summary>
public interface IPasswordHasherService
{
    /// <summary>
    /// Açık kullanıcı parolasından geri dönüştürülemeyen güvenli hash üretir.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Gönderilen açık parolanın kayıtlı parola hash'iyle eşleşmesini doğrular.
    /// </summary>
    bool VerifyPassword(
        string hashedPassword,
        string providedPassword);
}

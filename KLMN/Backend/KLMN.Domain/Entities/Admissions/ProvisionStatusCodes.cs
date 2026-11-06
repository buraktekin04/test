namespace KLMN.Domain.Entities.Admissions;

/// <summary>Provizyon durumlarının kayıt ve geçiş işlemlerindeki kodları.</summary>
public static class ProvisionStatusCodes
{
    public const int New = 1;
    public const int Report = 2;
    public const int Cancelled = 3;
    public const int Notification = 4;
}

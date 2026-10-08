namespace KLMN.Application.Common.Exceptions;

/// <summary>Kullanıcı hesabının kilitli olduğunu belirtir.</summary>
public sealed class AccountLockedException : Exception
{
    /// <summary>
    /// account locked exception işlemini ilgili servis sözleşmesine göre yürütür.
    /// </summary>
    public AccountLockedException(DateTime? lockoutEnd)
        : base(lockoutEnd.HasValue
            ? "Hesabınız geçici olarak kilitlenmiştir. Lütfen daha sonra tekrar deneyiniz."
            : "Hesabınız kilitlenmiştir. Sistem yöneticinizle iletişime geçiniz.")
    {
        LockoutEnd = lockoutEnd;
    }

    /// <summary>
    /// lockout end değerini ilgili API veya servis sözleşmesinde taşır.
    /// </summary>
    public DateTime? LockoutEnd { get; }
}

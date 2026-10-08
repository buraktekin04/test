namespace KLMN.Application.Common.Exceptions;

/// <summary>Kullanıcı hesabının kilitli olduğunu belirtir.</summary>
public sealed class AccountLockedException : Exception
{
    public AccountLockedException(DateTime? lockoutEnd)
        : base(lockoutEnd.HasValue
            ? "Hesabınız geçici olarak kilitlenmiştir. Lütfen daha sonra tekrar deneyiniz."
            : "Hesabınız kilitlenmiştir. Sistem yöneticinizle iletişime geçiniz.")
    {
        LockoutEnd = lockoutEnd;
    }

    public DateTime? LockoutEnd { get; }
}

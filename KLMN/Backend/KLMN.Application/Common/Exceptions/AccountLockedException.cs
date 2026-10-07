namespace KLMN.Application.Common.Exceptions;

/// <summary>
/// Kullanıcı hesabının kilitli olduğunu belirtir.
/// </summary>
public sealed class AccountLockedException : Exception
{
    public AccountLockedException()
        : base("Kullanıcı hesabı geçici olarak kilitlenmiştir.")
    {
    }
}

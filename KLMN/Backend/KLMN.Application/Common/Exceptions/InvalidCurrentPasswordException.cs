namespace KLMN.Application.Common.Exceptions;

/// <summary>Change password işleminde mevcut parolanın yanlış olduğunu belirtir.</summary>
public sealed class InvalidCurrentPasswordException : Exception
{
    /// <summary>
    /// invalid current password exception işlemini ilgili servis sözleşmesine göre yürütür.
    /// </summary>
    public InvalidCurrentPasswordException()
        : base("Mevcut parola hatalıdır.")
    {
    }
}

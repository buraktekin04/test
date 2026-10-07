namespace KLMN.Application.Common.Exceptions;

/// <summary>
/// Change password işleminde mevcut parolanın yanlış olduğunu belirtir.
/// </summary>
public sealed class InvalidCurrentPasswordException : Exception
{
    public InvalidCurrentPasswordException()
        : base("Mevcut parola hatalı.")
    {
    }
}

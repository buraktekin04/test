namespace KLMN.Application.Dispatches.Exceptions;

/// <summary>İş kuralı ve kimlik çakışmasında kullanılan özel hata.</summary>
public sealed class DispatchConflictException : Exception
{
    public DispatchConflictException(string message) : base(message) { }
}

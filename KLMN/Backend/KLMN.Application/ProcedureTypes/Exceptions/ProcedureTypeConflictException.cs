namespace KLMN.Application.ProcedureTypes.Exceptions;
/// <summary>Tetkik türü kodu/bağımlılık çakışması: HTTP 409.</summary>
public sealed class ProcedureTypeConflictException(string message):Exception(message);

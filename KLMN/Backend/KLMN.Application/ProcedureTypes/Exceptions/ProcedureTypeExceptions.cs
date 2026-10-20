namespace KLMN.Application.ProcedureTypes.Exceptions;
/// <summary>Tetkik türü bulunamadı: HTTP 404.</summary>
public sealed class ProcedureTypeNotFoundException():Exception("Tetkik Türü bulunamadı.");
/// <summary>Tetkik türü kodu/bağımlılık çakışması: HTTP 409.</summary>
public sealed class ProcedureTypeConflictException(string message):Exception(message);

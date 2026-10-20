namespace KLMN.Application.Procedures.Exceptions;

/// <summary>Bulunamayan veya silinmiş tetkik kaydı: HTTP 404.</summary>
public sealed class ProcedureNotFoundException()
    : Exception("Tetkik kaydı bulunamadı.");

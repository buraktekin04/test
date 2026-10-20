namespace KLMN.Application.ReportTypes.Exceptions;

/// <summary>Bulunamayan veya silinmiş rapor türü kaydı: HTTP 404.</summary>
public sealed class ReportTypeNotFoundException()
    : Exception("Rapor Türü kaydı bulunamadı.");

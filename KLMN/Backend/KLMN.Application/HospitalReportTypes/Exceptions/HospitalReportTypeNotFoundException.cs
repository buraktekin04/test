namespace KLMN.Application.HospitalReportTypes.Exceptions;

/// <summary>Bulunamayan veya silinmiş sağlık tesisi rapor türü kaydı: HTTP 404.</summary>
public sealed class HospitalReportTypeNotFoundException()
    : Exception("Sağlık Tesisi Rapor Türü kaydı bulunamadı.");

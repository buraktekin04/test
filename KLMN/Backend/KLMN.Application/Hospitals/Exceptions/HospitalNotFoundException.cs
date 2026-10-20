namespace KLMN.Application.Hospitals.Exceptions;

/// <summary>Bulunamayan veya silinmiş sağlık tesisi kaydı: HTTP 404.</summary>
public sealed class HospitalNotFoundException()
    : Exception("Sağlık Tesisi kaydı bulunamadı.");

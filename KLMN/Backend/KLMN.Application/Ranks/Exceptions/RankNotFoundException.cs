namespace KLMN.Application.Ranks.Exceptions;

/// <summary>Bulunamayan veya silinmiş rütbe kaydı: HTTP 404.</summary>
public sealed class RankNotFoundException()
    : Exception("Rütbe kaydı bulunamadı.");

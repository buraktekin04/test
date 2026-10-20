namespace KLMN.Application.RankGroups.Exceptions;

/// <summary>Bulunamayan veya silinmiş rütbe grubu kaydı: HTTP 404.</summary>
public sealed class RankGroupNotFoundException()
    : Exception("Rütbe Grubu kaydı bulunamadı.");

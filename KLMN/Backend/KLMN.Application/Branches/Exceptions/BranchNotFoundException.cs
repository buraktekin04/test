namespace KLMN.Application.Branches.Exceptions;

/// <summary>Bulunamayan veya silinmiş branş kaydı: HTTP 404.</summary>
public sealed class BranchNotFoundException()
    : Exception("Branş kaydı bulunamadı.");

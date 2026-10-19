namespace KLMN.Application.ConnectedDecisions.Exceptions;

/// <summary>Bulunamayan veya silinmiş karar kaydı: HTTP 404.</summary>
public sealed class ConnectedDecisionNotFoundException()
    : Exception("Bağlı kurul kararı kaydı bulunamadı.");

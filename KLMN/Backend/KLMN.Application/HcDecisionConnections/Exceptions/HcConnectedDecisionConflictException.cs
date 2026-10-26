namespace KLMN.Application.HcDecisionConnections.Exceptions;
/// <summary>Bağlı Kararlar için çakışan kayıt veya pasif referans.</summary>
public sealed class HcConnectedDecisionConflictException(string message):Exception(message);

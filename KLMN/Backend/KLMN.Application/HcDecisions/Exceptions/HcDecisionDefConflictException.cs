namespace KLMN.Application.HcDecisions.Exceptions;
/// <summary>Kurul kararı kodu veya ilişkili kayıt iş kuralı ihlali.</summary>
public sealed class HcDecisionDefConflictException(string message):Exception(message);

namespace KLMN.Application.HcUnusedDecisions.Exceptions;
/// <summary>Birlikte Verilmeyecek Kararlar için çakışan kayıt veya pasif referans.</summary>
public sealed class HcUnusedDecisionConflictException(string message):Exception(message);

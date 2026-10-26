namespace KLMN.Application.HcDecisions.Exceptions;
/// <summary>Kurul kararının silinmiş veya bulunamamış olduğunu bildirir.</summary>
public sealed class HcDecisionDefNotFoundException():Exception("Kurul kararı bulunamadı.");

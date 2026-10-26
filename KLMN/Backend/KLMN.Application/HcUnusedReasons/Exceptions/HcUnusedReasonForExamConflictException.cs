namespace KLMN.Application.HcUnusedReasons.Exceptions;
/// <summary>Kullanılmayacak Muayene Sebepleri için çakışan kayıt veya pasif referans.</summary>
public sealed class HcUnusedReasonForExamConflictException(string message):Exception(message);

namespace KLMN.Application.HcUnusedAdmissionTypes.Exceptions;
/// <summary>Kullanılmayacak Başvuru Grupları için çakışan kayıt veya pasif referans.</summary>
public sealed class HcUnusedAdmissionTypeConflictException(string message):Exception(message);

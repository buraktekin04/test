namespace KLMN.Application.AdmissionTypes.Exceptions;

/// <summary>Silinmiş veya bulunamayan başvuru grubuna erişim hatası.</summary>
public sealed class AdmissionTypeNotFoundException()
    : Exception("Başvuru grubu kaydı bulunamadı.");
